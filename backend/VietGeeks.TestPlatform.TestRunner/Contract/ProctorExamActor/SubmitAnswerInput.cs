namespace VietGeeks.TestPlatform.TestRunner.Contract.ProctorExamActor;

public class SubmitAnswerInput
{
    public string[] AnswerIds { get; set; } = default!;
    public string QuestionId { get; set; } = default!;
}