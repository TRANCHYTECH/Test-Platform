using System.Collections.Generic;

namespace VietGeeks.TestPlatform.TestRunner.Contract.ProctorExamActor;

public class FinishExamOutput: IExamStepInfo
{
    public decimal FinalMark { get; set; }
    public List<AggregatedGrading> Grading { get; set; } = default!;
    public ExamStep Step { get; set; }
}

public class AggregatedGrading
{
    public int GradingType { get; set; }

    public bool? PassMark { get; set; }

    public Dictionary<string, string>? Grades { get; set; }
}