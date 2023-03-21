using Dapr.Actors.Runtime;
using VietGeeks.TestPlaftorm.TestRunner.Infrastructure.Services;
using VietGeeks.TestPlatform.SharedKernel.Exceptions;
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

    public async Task<StartExamOutput> StartExam(StartExamInput input)
    {
        //todo: implement logic if allow to start exam again or not.If not, check exam already started, then fail.
        // also exam finished.
        //todo: improve logic to get exam content. store in separarte storage within test runner scope? Now questions in exam and exam state might be mistached.
        var examContent = await _proctorService.GenerateExamContent(new()
        {
            ExamId = input.ExamId,
            TestDefinitionId = input.TestDefinitionId
        });

        var examState = await GetExamState();
        if (examState == null)
        {
            examState = new ExamState
            {
                ExamId = input.ExamId,
                TestDefinitionId = input.TestDefinitionId,
                Questions = examContent.Questions.ToDictionary(c => c.Id, d => d.Answers.Select(a => a.Id).ToArray()),
                StartedAt = DateTime.UtcNow
            };

            await SaveExamState(examState);
        }

        return new()
        {
            StartedAt = examState.StartedAt,
            Questions = examContent.Questions,
            TestDuration = examContent.TestDuration
        };
    }

    public async Task SubmitAnswer(SubmitAnswerInput input)
    {
        //todo: refactor the way to store questions in order to validate whether or not the answer is valid in configured timespan.
        await ExamStateAction(examState =>
        {
            if (!examState.Questions.ContainsKey(input.QuestionId))
            {
                throw new TestPlatformException("NotFoundQuestion");
            }

            if (examState.Answers.ContainsKey(input.QuestionId))
            {
                throw new TestPlatformException("AlreadyAnswered");
            }

            examState.Answers[input.QuestionId] = input.AnswerIds;
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

    private async Task SaveExamState(ExamState examState)
    {
        await StateManager.SetStateAsync(ExamStateName, examState);
    }

    private class ExamState
    {
        public string ExamId { get; set; } = default!;

        public string TestDefinitionId { get; set; } = default!;

        public Dictionary<string, string[]> Questions { get; set; } = default!;

        public Dictionary<string, string[]> Answers { get; set; } = new Dictionary<string, string[]>();

        public DateTime StartedAt { get; set; }

        public DateTime? FinishedAt { get; set; }
    }
}
