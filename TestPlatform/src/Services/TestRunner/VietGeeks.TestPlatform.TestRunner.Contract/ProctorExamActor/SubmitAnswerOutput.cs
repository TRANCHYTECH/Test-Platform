namespace VietGeeks.TestPlatform.TestRunner.Contract.ProctorExamActor;


public class SubmitAnswerOutput: IActiveQuestion
{
    public string? ActiveQuestionId { get; set; }
    public int? ActiveQuestionIndex { get; set; }
    public ExamQuestion? ActiveQuestion { get; set; }
}