using System.Collections.Generic;

namespace VietGeeks.TestPlatform.TestRunner.Contract.ProctorExamActor;

public class ExamStatus
{
    public ExamStep PreviousStep { get;set; }
    public string ExamId { get; set; } = default!;
    public ExamQuestion? ActiveQuestion { get; set; }
    
    public Dictionary<string, string> ExamineeInfo { get; set; } = new Dictionary<string, string>();
}