using Dapr.Actors.Runtime;
using VietGeeks.TestPlaftorm.TestRunner.Infrastructure.Services;
using VietGeeks.TestPlatform.SharedKernel.Exceptions;
using VietGeeks.TestPlatform.TestRunner.Contract;
using VietGeeks.TestPlatform.TestRunner.Contract.ProctorExamActor;

namespace VietGeeks.TestPlatform.TestRunner.Api.Actors;

public class ProctorActor : Actor, IProctorActor
{
    private const string ExamStateName = "Exam";
    private readonly IProctorService _proctorService;

    public ProctorActor(ActorHost host, IProctorService proctorService) : base(host)
    {
        _proctorService = proctorService;
    }

    public async Task<string> ProvideExamineeInfo(ProvideExamineeInfoInput input)
    {
        var examId = await _proctorService.ProvideExamineeInfo(input);
        ExamState examState = CreateExamState(examId);
        examState.ExamineeInfo = input.ExamineeInfo;
        examState.TestRunId = input.TestRunId;
        await SaveExamState(examState);

        return examId;
    }

    public async Task<StartExamOutput> StartExam(StartExamInput input)
    {
        var examContent = await _proctorService.GenerateExamContent(new()
        {
            ExamId = input.ExamId
        });

        var examState = await GetExamState();
        examState.QuestionIds = examContent.Questions.Select(q => q.Id).ToArray();
        examState.ActiveQuestionIndex = 0;
        examState.ActiveQuestionId = examContent.ActiveQuestion?.Id;
        examState.StartedAt = DateTime.UtcNow;

        if (!string.IsNullOrEmpty(examState.ActiveQuestionId)) {
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

        await SaveExamState(examState);

        return new()
        {
            StartedAt = examState.StartedAt,
            Questions = examContent.Questions,
            TestDuration = examContent.TestDuration,
            ActiveQuestion = examContent.ActiveQuestion,
            ActiveQuestionIndex = examState.ActiveQuestionIndex,
            TotalQuestion = examState.QuestionIds.Length
        };
    }

    public async Task<SubmitAnswerOutput> SubmitAnswer(SubmitAnswerInput input)
    {
        //todo: refactor the way to store questions in order to validate whether or not the answer is valid in configured timespan.
        return await ExamStateAction(async examState =>
        {
            if (!examState.QuestionIds.Contains(input.QuestionId))
            {
                throw new TestPlatformException("NotFoundQuestion");
            }

            if (examState.Answers.ContainsKey(input.QuestionId))
            {
                throw new TestPlatformException("AlreadyAnswered");
            }

            if (!examState.QuestionTimes.ContainsKey(input.QuestionId))
            {
                throw new TestPlatformException("QuestionIsNotStarted");
            }

            examState.Answers[input.QuestionId] = input.AnswerIds;
            examState.QuestionTimes[input.QuestionId].SubmittedAt = DateTime.UtcNow;
            var questionIndex = Array.IndexOf(examState.QuestionIds, input.QuestionId);
            string? activeQuestionId = null;
            ExamQuestion? activeQuestion = null;
            if (questionIndex == examState.QuestionIds.Length - 1)
            {
                examState.ActiveQuestionIndex = null;
            }
            else
            {
                examState.ActiveQuestionIndex = questionIndex + 1;
                activeQuestionId = examState.QuestionIds[examState.ActiveQuestionIndex.Value];
                activeQuestion = await _proctorService.GetTestRunQuestion(examState.ExamId, activeQuestionId);
                examState.QuestionTimes[activeQuestionId] = new QuestionTiming
                {
                    StartedAt = DateTime.UtcNow
                };
            }
            examState.ActiveQuestionId = activeQuestionId;

            return new SubmitAnswerOutput()
            {
                ActiveQuestionId = activeQuestionId,
                ActiveQuestionIndex = examState.ActiveQuestionIndex,
                ActiveQuestion = activeQuestion
            };
        });
    }

    public async Task<FinishExamOutput> FinishExam()
    {
        var result = await ExamStateAction(async examState =>
        {
            examState.FinishedAt = DateTime.UtcNow;
            var output = await _proctorService.FinishExam(new()
            {
                ExamId = examState.ExamId,
                Answers = examState.Answers,
                StartedAt = examState.StartedAt,
                FinishededAt = examState.FinishedAt.GetValueOrDefault()
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
            activeQuestion = await _proctorService.GetTestRunQuestion(examState.ExamId, examState.ActiveQuestionId);
            activeQuestionStartedAt = examState.QuestionTimes[examState.ActiveQuestionId]?.StartedAt;
        }

        var testRun = await this._proctorService.GetTestRun(examState.TestRunId);

        return new ExamStatus()
        {
            TestName = testRun.TestDefinitionSnapshot?.BasicSettings?.Name ?? string.Empty,
            StartedAt = examState.StartedAt,
            FinishededAt = examState.FinishedAt,
            ActiveQuestion = activeQuestion,
            ActiveQuestionIndex = examState.ActiveQuestionIndex,
            ActiveQuestionStartedAt = activeQuestionStartedAt,
            QuestionCount = examState.QuestionIds?.Length ?? 0,
            ExamineeInfo = examState.ExamineeInfo,
            TestDuration = new TestDuration 
            {
                Duration = examState.TestDuration.Duration,
                Method = (TestDurationMethodType) examState.TestDuration.Method
            },
            Grading = examState.Grading
        };
    }

    private async Task<T> ExamStateAction<T>(Func<ExamState, Task<T>> action)
    {
        ExamState examState = await GetExamState();
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
        var state = await StateManager.TryGetStateAsync<ExamState>(ExamStateName);

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
        await StateManager.SetStateAsync(ExamStateName, examState);
    }

    private class ExamState
    {
        public string ExamId { get; set; } = default!;
        public string TestRunId { get; set; } = default!;
        public string TestName { get; set; } = default!;

        public string TestDefinitionId { get; set; } = default!;

        public string[] QuestionIds { get; set; } = default!;
        public string? ActiveQuestionId { get; set; } = default!;
        public int? ActiveQuestionIndex { get; set; } = default!;
        public TestDurationState TestDuration { get; set; } = default!;
        public Dictionary<string, string> ExamineeInfo { get; set; } = new Dictionary<string, string>();

        public Dictionary<string, string[]> Answers { get; set; } = new Dictionary<string, string[]>();

        public Dictionary<string, QuestionTiming> QuestionTimes {get;set;} = new Dictionary<string, QuestionTiming>();
        public IEnumerable<AggregatedGrading> Grading { get; set; } = default!;

        public DateTime StartedAt { get; set; }

        public DateTime? FinishedAt { get; set; }
    }

    private class TestDurationState
    {
        public int Method { get; set; }

        public TimeSpan Duration { get; set; }
    }

    private class QuestionTiming 
    {
        public DateTime StartedAt {get;set;}
        public DateTime? SubmittedAt {get;set;}
    }
}
