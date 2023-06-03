﻿using Dapr.Actors.Runtime;
using Microsoft.Extensions.Caching.Memory;
using VietGeeks.TestPlaftorm.TestRunner.Infrastructure.Services;
using VietGeeks.TestPlatform.TestManager.Core.Models;
using VietGeeks.TestPlatform.TestRunner.Contract;
using VietGeeks.TestPlatform.TestRunner.Contract.ProctorExamActor;

namespace VietGeeks.TestPlatform.TestRunner.Api.Actors;

public class ProctorActor : Actor, IProctorActor
{
    private const string EXAM_STATE_NAME = "Exam";
    private const string TEST_QUESTIONS_CACHE_KEY = "Exam_TestQuestions";
    private readonly IProctorService _proctorService;
    private readonly IMemoryCache _memoryCache;

    public ProctorActor(ActorHost host, IProctorService proctorService, IMemoryCache memoryCache) : base(host)
    {
        _proctorService = proctorService;
        _memoryCache = memoryCache;
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

        return new()
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

            var output = new SubmitAnswerOutput()
            {
                IsSuccess = true
            };

            if (activeQuestion?.ScoreSettings?.IsMandatory == true)
            {
                output.Terminated = !_proctorService.IsCorrectAnswer(activeQuestion, input.AnswerIds);
            }

            return output;
        });
    }

    private async Task<string> ValidateSubmissionAsync(SubmitAnswerInput input, ExamState examState, QuestionDefinition? activeQuestion)
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

        if (activeQuestion?.ScoreSettings?.MustAnswerToContinue == true && (input.AnswerIds == null || !input.AnswerIds.Any()))
        {
            return "MustAnswerToContinue";
        }

        return string.Empty;
    }

    public async Task<ActivateQuestionOutput> ActivateNextQuestion(ActivateNextQuestionInput input)
    {
        return await ExamStateAction(async examState =>
        {
            var questionIndex = Array.IndexOf(examState.QuestionIds, examState.ActiveQuestionId);
            var offset = input.Direction == ActivateDirection.Previous ? -1 : 1;
            var nextQuestionIndex = questionIndex + offset;

            return await ActivateQuestion(nextQuestionIndex);
        });
    }

    public async Task<ActivateQuestionOutput> ActivateQuestion(int nextQuestionIndex)
    {
        return await ExamStateAction(async examState =>
        {
            string[]? answers = null;

            if (nextQuestionIndex < 0 || nextQuestionIndex >= examState.QuestionIds.Length)
            {
                return new ActivateQuestionOutput()
                {
                    ActivationResult = false
                };
            }
            else
            {
                examState.ActiveQuestionIndex = nextQuestionIndex;
                examState.ActiveQuestionId = examState.QuestionIds[examState.ActiveQuestionIndex.Value];
                if (!examState.QuestionTimes.ContainsKey(examState.ActiveQuestionId))
                {
                    examState.QuestionTimes[examState.ActiveQuestionId] = new QuestionTiming
                    {
                        StartedAt = DateTime.UtcNow
                    };
                }
                if (examState.CanSkipQuestion && examState.Answers.TryGetValue(examState.ActiveQuestionId, out var previousAnswers))
                {
                    answers = previousAnswers;
                }
            }

            return new ActivateQuestionOutput()
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
            var output = await _proctorService.FinishExam(new()
            {
                ExamId = examState.ExamId,
                Answers = examState.Answers,
                QuestionTimes = examState.QuestionTimes.ToDictionary(c => c.Key, c => new[] { c.Value.StartedAt, c.Value.SubmittedAt }),
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

    private async Task<QuestionDefinition?> GetActiveQuestionAsync(ExamState examState)
    {
        var testRunQuestions = await _memoryCache.GetOrCreateAsync(TEST_QUESTIONS_CACHE_KEY,
        (entry) =>
        {
            entry.SlidingExpiration = TimeSpan.FromHours(1);
            return _proctorService.GetTestRunQuestionsByExamId(examState.ExamId);
        });

        return testRunQuestions?.SingleOrDefault(q => q.ID == examState.ActiveQuestionId);
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
        public string? ActiveQuestionId { get; set; } = default!;
        public int? ActiveQuestionIndex { get; set; } = default!;
        public TestDurationState TestDuration { get; set; } = default!;
        public Dictionary<string, string> ExamineeInfo { get; set; } = new Dictionary<string, string>();

        public Dictionary<string, string[]> Answers { get; set; } = new Dictionary<string, string[]>();

        public Dictionary<string, QuestionTiming> QuestionTimes { get; set; } = new Dictionary<string, QuestionTiming>();
        public IEnumerable<AggregatedGradingOuput> Grading { get; set; } = default!;

        public DateTime StartedAt { get; set; }

        public DateTime? FinishedAt { get; set; }

        public bool CanSkipQuestion { get; set; }

        //todo: better place
        public TimeSpan TotalDuration()
        {
            return TestDuration.Method == (int)TestDurationMethodType.CompleteTestTime ? TestDuration.Duration : QuestionIds.Length * TestDuration.Duration;
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
