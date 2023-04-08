namespace VietGeeks.TestPlatform.TestRunner.Contract.ProctorExamActor;


public class SubmitAnswerOutput: IExamStepInfo
{
    public string? ActiveQuestionId { get; set; }
    public ExamQuestion? ActiveQuestion { get; set; }
    public ExamStep Step { get; set; }
}

