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
        examState.StartedAt = DateTime.UtcNow;

        await SaveExamState(examState);

        return new()
        {
            StartedAt = examState.StartedAt,
            Questions = examContent.Questions,
            TestDuration = examContent.TestDuration,
            ActiveQuestion = examContent.ActiveQuestion
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

            examState.Answers[input.QuestionId] = input.AnswerIds;
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
            }
            examState.ActiveQuestionId = activeQuestionId;

            return new SubmitAnswerOutput()
            {
                ActiveQuestionId = activeQuestionId,
                ActiveQuestion = activeQuestion
            };
        });
    }

    public async Task<FinishExamOutput> FinishExam()
    {
        var result = await ExamStateAction(async examState =>
        {
            examState.FinishedAt = DateTime.UtcNow;
            return await _proctorService.FinishExam(new()
            {
                ExamId = examState.ExamId,
                Answers = examState.Answers,
                StartedAt = examState.StartedAt,
                FinishededAt = examState.FinishedAt.GetValueOrDefault()
            });
        });

        return result;
    }

    public async Task<ExamStatus> GetExamStatus()
    {
        var examState = await GetExamState();
        ExamQuestion? activeQuestion = null;
        if (examState.ActiveQuestionIndex.HasValue)
        {
            activeQuestion = await _proctorService.GetTestRunQuestion(examState.ExamId, examState.ActiveQuestionId);
        }

        return new ExamStatus()
        {
            ActiveQuestion = activeQuestion,
            ExamineeInfo = examState.ExamineeInfo
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

        public string TestDefinitionId { get; set; } = default!;

        public string[] QuestionIds { get; set; } = default!;
        public string? ActiveQuestionId { get; set; } = default!;
        public int? ActiveQuestionIndex { get; set; } = default!;
        public Dictionary<string, string> ExamineeInfo { get; set; } = new Dictionary<string, string>();

        public Dictionary<string, string[]> Answers { get; set; } = new Dictionary<string, string[]>();

        public DateTime StartedAt { get; set; }

        public DateTime? FinishedAt { get; set; }
    }
}
