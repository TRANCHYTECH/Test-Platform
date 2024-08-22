using Dapr.Actors.Runtime;
using VietGeeks.TestPlatform.TestManager.Data.Models;
using VietGeeks.TestPlatform.TestRunner.Contract;
using VietGeeks.TestPlatform.TestRunner.Contract.ProctorExamActor;
using VietGeeks.TestPlatform.TestRunner.Infrastructure.MapperProfiles;
using VietGeeks.TestPlatform.TestRunner.Infrastructure.Services;

namespace VietGeeks.TestPlatform.TestRunner.Api.Actors;

public class ProctorActor(ActorHost host, IProctorService proctorService) : Actor(host), IProctorActor
{
    private const string EXAM_STATE_NAME = "Exam";
    private const int SUBMIT_ALLOW_WINDOW_SECONDS = 5;

    private List<QuestionDefinition>? _testRunQuestions;

    public async Task<string> ProvideExamineeInfo(ProvideExamineeInfoInput input)
    {
        var examId = await proctorService.ProvideExamineeInfo(input);
        var examState = CreateExamState(examId);
        examState.ExamineeInfo = input.ExamineeInfo;
        examState.TestRunId = input.TestRunId;
        await SaveExamState(examState);

        return examId;
    }

    public async Task<StartExamOutput> StartExam(StartExamInput input)
    {
        var examContent = await proctorService.GenerateExamContent(new GenerateExamContentInput
        {
            ExamId = input.ExamId
        });

        var examState = await GetExamState();
        examState.QuestionIds = examContent.Questions.Select(q => q.Id).ToArray();
        examState.ActiveQuestionIndex = 0;
        examState.ActiveQuestionId = examContent.ActiveQuestion?.Id;
        examState.StartedAt = DateTime.UtcNow;
        examState.CanSkipQuestion = examContent.CanSkipQuestion;
        if (!string.IsNullOrEmpty(examState.ActiveQuestionId))
        {
            examState.QuestionTimes[examState.ActiveQuestionId] = new QuestionTiming
            {
                StartedAt = DateTime.UtcNow
            };
        }

        examState.TestDuration = new TestDurationState
        {
            Duration = examContent.TestDuration.Duration,
            Method = (int)examContent.TestDuration.Method
        };
        examState.TestDuration.TotalDuration = examState.TotalDuration();

        await SaveExamState(examState);

        return new StartExamOutput
        {
            StartedAt = examState.StartedAt,
            TestDuration = examContent.TestDuration,
            ActiveQuestion = examContent.ActiveQuestion,
            ActiveQuestionIndex = examState.ActiveQuestionIndex,
            TotalQuestion = examState.QuestionIds.Length,
            CanSkipQuestion = examState.CanSkipQuestion,
            TotalDuration = examState.TotalDuration()
        };
    }

    public async Task<SubmitAnswerOutput> SubmitAnswer(SubmitAnswerInput input)
    {
        //todo: refactor the way to store questions in order to validate whether or not the answer is valid in configured timespan.
        return await ExamStateAction(async examState =>
        {
            var activeQuestion = await GetActiveQuestionAsync(examState);
            var validationError = await ValidateSubmissionAsync(input, examState, activeQuestion);

            if (!string.IsNullOrEmpty(validationError))
            {
                return new SubmitAnswerOutput
                {
                    IsSuccess = false,
                    Reason = validationError,
                    Terminated = false
                };
            }

            examState.Answers[input.QuestionId] = input.AnswerIds;
            examState.QuestionTimes[input.QuestionId].SubmittedAt = DateTime.UtcNow;

            var output = new SubmitAnswerOutput
            {
                IsSuccess = true
            };

            if (activeQuestion?.ScoreSettings?.IsMandatory == true)
            {
                output.Terminated = !proctorService.IsCorrectAnswer(activeQuestion, input.AnswerIds);
            }

            return output;
        });
    }

    public async Task<ActivateQuestionOutput> ActivateQuestionFromCurrent(ActivateNextQuestionInput input)
    {
        return await ExamStateAction(async examState =>
        {
            var questionIndex = Array.IndexOf(examState.QuestionIds, examState.ActiveQuestionId);
            var offset = input.Direction == ActivateDirection.Previous ? -1 : 1;
            var nextQuestionIndex = questionIndex + offset;

            return await ActivateQuestionByIndex(nextQuestionIndex);
        });
    }

    public async Task<ActivateQuestionOutput> ActivateQuestionByIndex(int nextQuestionIndex)
    {
        return await ExamStateAction(async examState =>
        {
            string[]? answers = null;

            if (nextQuestionIndex < 0 || nextQuestionIndex >= examState.QuestionIds.Length)
            {
                return new ActivateQuestionOutput
                {
                    ActivationResult = false
                };
            }

            examState.ActiveQuestionIndex = nextQuestionIndex;
            examState.ActiveQuestionId = examState.QuestionIds[examState.ActiveQuestionIndex.Value];
            if (!examState.QuestionTimes.ContainsKey(examState.ActiveQuestionId))
            {
                examState.QuestionTimes[examState.ActiveQuestionId] = new QuestionTiming
                {
                    StartedAt = DateTime.UtcNow
                };
            }

            if (examState.CanSkipQuestion &&
                examState.Answers.TryGetValue(examState.ActiveQuestionId, out var previousAnswers))
            {
                answers = previousAnswers;
            }

            return new ActivateQuestionOutput
            {
                ActiveQuestionId = examState.ActiveQuestionId,
                ActiveQuestionIndex = examState.ActiveQuestionIndex,
                ActiveQuestion = (await GetActiveQuestionAsync(examState))?.ToViewModel(),
                AnswerIds = answers,
                CanFinish = examState.CanSkipQuestion && examState.Answers.Count == examState.QuestionIds?.Length,
                ActivationResult = true
            };
        });
    }

    public async Task<FinishExamOutput> FinishExam()
    {
        var result = await ExamStateAction(async examState =>
        {
            examState.FinishedAt = DateTime.UtcNow;
            var output = await proctorService.FinishExam(new FinishExamInput
            {
                ExamId = examState.ExamId,
                Answers = examState.Answers,
                QuestionTimes = examState.QuestionTimes.ToDictionary(c => c.Key,
                    c => new[] { c.Value.StartedAt, c.Value.SubmittedAt }),
                StartedAt = examState.StartedAt,
                FinishededAt = examState.FinishedAt.GetValueOrDefault(),
                TotalDuration = examState.TotalDuration()
            });
            examState.Grading = output.Grading;

            return output;
        });

        return result;
    }

    public async Task<ExamStatus> GetExamStatus()
    {
        var examState = await GetExamState();
        ExamQuestion? activeQuestion = null;
        DateTime? activeQuestionStartedAt = null;

        if (!string.IsNullOrEmpty(examState.ActiveQuestionId))
        {
            activeQuestion = (await GetActiveQuestionAsync(examState))?.ToViewModel();
            activeQuestionStartedAt = examState.QuestionTimes[examState.ActiveQuestionId]?.StartedAt;
        }

        var testRun = await proctorService.GetTestRun(examState.TestRunId);

        return new ExamStatus
        {
            TestName = testRun.TestDefinitionSnapshot?.BasicSettings?.Name ?? string.Empty,
            StartedAt = examState.StartedAt,
            FinishededAt = examState.FinishedAt,
            ActiveQuestion = activeQuestion,
            ActiveQuestionIndex = examState.ActiveQuestionIndex,
            ActiveQuestionStartedAt = activeQuestionStartedAt,
            QuestionCount = examState.QuestionIds?.Length ?? 0,
            CanSkipQuestion = examState.CanSkipQuestion,
            ExamineeInfo = examState.ExamineeInfo,
            TestDuration = new TestDuration
            {
                Duration = examState.TestDuration.Duration,
                Method = (TestDurationMethodType)examState.TestDuration.Method
            },
            TotalDuration = examState.TotalDuration(),
            Grading = examState.Grading,
            CanFinish = examState.CanSkipQuestion && examState.Answers.Count == examState.QuestionIds?.Length
        };
    }

    private async Task<string> ValidateSubmissionAsync(SubmitAnswerInput input, ExamState examState,
        QuestionDefinition? activeQuestion)
    {
        if (!examState.QuestionIds.Contains(input.QuestionId))
        {
            return "NotFoundQuestion";
        }

        if (!examState.CanSkipQuestion && examState.Answers.ContainsKey(input.QuestionId))
        {
            return "AlreadyAnswered";
        }

        if (!examState.QuestionTimes.ContainsKey(input.QuestionId))
        {
            return "QuestionIsNotStarted";
        }

        var startTime = examState.TestDuration.Method == (int)TestDurationMethodType.CompleteTestTime
            ? examState.StartedAt
            : examState.QuestionTimes[input.QuestionId].StartedAt;

        if (examState.TestDuration.TotalDuration <=
            DateTime.UtcNow - startTime - TimeSpan.FromSeconds(SUBMIT_ALLOW_WINDOW_SECONDS))
        {
            return "TimeUp";
        }

        if (activeQuestion?.ScoreSettings?.MustAnswerToContinue == true &&
            (input.AnswerIds == null || !input.AnswerIds.Any()))
        {
            return "MustAnswerToContinue";
        }

        return string.Empty;
    }

    private async Task<QuestionDefinition?> GetActiveQuestionAsync(ExamState examState)
    {
        if (_testRunQuestions == null)
        {
            _testRunQuestions = (await proctorService.GetTestRunQuestionsByExamId(examState.ExamId)).ToList();
        }

        return _testRunQuestions?.SingleOrDefault(q => q.ID == examState.ActiveQuestionId);
    }

    private async Task<T> ExamStateAction<T>(Func<ExamState, Task<T>> action)
    {
        var examState = await GetExamState();
        var result = await action(examState);
        await SaveExamState(examState);

        return result;
    }

    private async Task ExamStateAction(Action<ExamState> action)
    {
        var examState = await GetExamState();
        action(examState);
        await SaveExamState(examState);
    }

    private async Task<ExamState> GetExamState()
    {
        var state = await StateManager.TryGetStateAsync<ExamState>(EXAM_STATE_NAME);

        return state.Value;
    }

    private static ExamState CreateExamState(string examId)
    {
        return new ExamState
        {
            ExamId = examId
        };
    }

    private async Task SaveExamState(ExamState examState)
    {
        await StateManager.SetStateAsync(EXAM_STATE_NAME, examState);
    }

    private class ExamState
    {
        public string ExamId { get; set; } = default!;
        public string TestRunId { get; set; } = default!;
        public string TestName { get; set; } = default!;

        public string TestDefinitionId { get; set; } = default!;

        public string[] QuestionIds { get; set; } = default!;
        public string? ActiveQuestionId { get; set; }
        public int? ActiveQuestionIndex { get; set; }
        public TestDurationState TestDuration { get; set; } = default!;
        public IDictionary<string, string> ExamineeInfo { get; set; } = new Dictionary<string, string>();

        public IDictionary<string, string[]> Answers { get; } = new Dictionary<string, string[]>();

        public IDictionary<string, QuestionTiming> QuestionTimes { get; } =
            new Dictionary<string, QuestionTiming>();

        public IEnumerable<AggregatedGradingOuput> Grading { get; set; } = default!;

        public DateTime StartedAt { get; set; }

        public DateTime? FinishedAt { get; set; }

        public bool CanSkipQuestion { get; set; }

        //todo: better place
        public TimeSpan TotalDuration()
        {
            return TestDuration.Method == (int)TestDurationMethodType.CompleteTestTime
                ? TestDuration.Duration
                : QuestionIds.Length * TestDuration.Duration;
        }
    }

    private class TestDurationState
    {
        public int Method { get; set; }

        public TimeSpan Duration { get; set; }

        public TimeSpan TotalDuration { get; set; }
    }

    private class QuestionTiming
    {
        public DateTime StartedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
    }
}