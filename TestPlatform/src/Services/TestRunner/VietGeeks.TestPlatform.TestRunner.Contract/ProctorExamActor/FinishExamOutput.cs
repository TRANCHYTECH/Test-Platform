using System.Collections.Generic;

namespace VietGeeks.TestPlatform.TestRunner.Contract.ProctorExamActor;

public class FinishExamOutput
{
    public decimal FinalMark { get; set; }
    public List<AggregatedGrading> Grading { get; set; } = default!;
}

public class AggregatedGrading
{
    public int GradingType { get; set; }

    public bool? PassMark { get; set; }

    public Dictionary<string, string>? Grades { get; set; }
}