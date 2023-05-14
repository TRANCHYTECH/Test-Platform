
using System.Collections.Generic;
using VietGeeks.TestPlatform.TestManager.Core.Models;

namespace VietGeeks.TestPlatform.TestManager.Core.ReadonlyModels;
public class AggregatedGrading
{
    public GradingCriteriaConfigType GradingType { get; set; }
    public Dictionary<string, string>? Grades { get; set; }
    public PassMarkGrade? PassMarkGrade {get;set;}
}

public class PassMarkGrade
{
    public bool? IsPass { get; set; }
    public decimal? FinalPoints { get; set; }
    public decimal? TotalPoints { get; set; }
    public decimal? PassValue {get;set;}
    public RangeUnit Unit { get; set; }
}