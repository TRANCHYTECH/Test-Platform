using Dapr.Actors.Runtime;
using VietGeeks.TestPlatform.TestRunner.Actors.Interfaces;
using VietGeeks.TestPlatform.TestRunner.Actors.Interfaces.Contracts;

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
                //todo: shared kernel exception.
                throw new Exception("Not found question");
            }

            if (!examState.Answers.ContainsKey(input.AnswerId))
            {
                //todo: shared kernel exception.
                throw new Exception("Already answsered");
            }

            examState.Answers[input.QuestionId] = input.AnswerId;
        });
    }

    private async Task ExamStateAction(Action<ExamState> action)
    {
        var examState = await StateManager.GetStateAsync<ExamState>(ExamStateName);
        if (examState == null)
        {
            //todo(tau): use shared kernel exception.
            throw new Exception("Not found state");
        }

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
