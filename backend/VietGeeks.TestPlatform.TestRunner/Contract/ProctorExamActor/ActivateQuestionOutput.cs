namespace VietGeeks.TestPlatform.TestRunner.Contract.ProctorExamActor;

public class ActivateQuestionOutput : IActiveQuestion
{
    public bool ActivationResult { get; set; }
    public string? ActiveQuestionId { get; set; }
    public bool CanFinish { get; set; }
    public string[]? AnswerIds { get; set; } = default!;
    public int? ActiveQuestionIndex { get; set; }
    public ExamQuestion? ActiveQuestion { get; set; }
}