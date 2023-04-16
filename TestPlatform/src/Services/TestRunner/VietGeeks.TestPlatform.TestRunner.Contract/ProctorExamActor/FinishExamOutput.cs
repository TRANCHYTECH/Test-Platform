using System;
using System.Collections.Generic;

namespace VietGeeks.TestPlatform.TestRunner.Contract.ProctorExamActor;

public class FinishExamOutput
{
    public decimal FinalMark { get; set; }
    public List<AggregatedGrading> Grading { get; set; } = default!;
    public DateTime FinishedAt { get; set; }
}

public class AggregatedGrading
{
    public int GradingType { get; set; }
    public PassMarkGrade? PassMarkGrade {get;set;}
    public Dictionary<string, string>? Grades { get; set; }
}

public class PassMarkGrade
{
    public bool? IsPass { get; set; }
    public decimal? FinalPoints { get; set; }
    public decimal? TotalPoints { get; set; }
    public decimal? PassValue {get;set;}
    public int Unit { get; set; }
}