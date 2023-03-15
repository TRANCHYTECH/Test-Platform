using Dapr.Actors.Runtime;
using VietGeeks.TestPlatform.TestRunner.Actors.Interfaces;
using VietGeeks.TestPlatform.TestRunner.Actors.Interfaces.Contracts;
using VietGeeks.TestPlatform.SharedKernel.Exceptions;

namespace VietGeeks.TestPlatform.TestRunner.Api.Actors;

public class ProctorActor : Actor, IProctorActor
{
    private const string ExamStateName = "Exam";

    public ProctorActor(ActorHost host) : base(host)
    {
    }

    public async Task StartExam(StartExamInput input)
    {
        var examState = new ExamState
        {
            ExamId = input.ExamId,
            TestDefinitionId = input.TestDefinitionId,
            Questions = input.Questions
        };

        await StateManager.SetStateAsync(ExamStateName, examState);
    }

    public async Task SubmitAnswer(SubmitAnswerInput input)
    {
        await ExamStateAction(examState =>
        {
            if (!examState.Questions.ContainsKey(input.QuestionId))
            {
                throw new TestPlatformException("NotFoundQuestion");
            }

            if (!examState.Answers.ContainsKey(input.AnswerId))
            {
                throw new TestPlatformException("AlreadyAnswered");
            }

            examState.Answers[input.QuestionId] = input.AnswerId;
        });
    }

    private async Task ExamStateAction(Action<ExamState> action)
    {
        var examState = await StateManager.GetStateAsync<ExamState>(ExamStateName) ?? throw new TestPlatformException("NotFoundExam");
        action(examState);
        await StateManager.SetStateAsync(ExamStateName, examState);
    }


    private class ExamState
    {
        public string ExamId { get; set; } = default!;

        public string TestDefinitionId { get; set; } = default!;

        public Dictionary<string, string[]> Questions { get; set; } = default!;

        public Dictionary<string, string> Answers { get; set; } = new Dictionary<string, string>();
    }
}
