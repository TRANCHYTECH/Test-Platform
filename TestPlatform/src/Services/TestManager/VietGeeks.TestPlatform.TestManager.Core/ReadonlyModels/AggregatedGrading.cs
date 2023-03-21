
using System.Collections.Generic;
using VietGeeks.TestPlatform.TestManager.Core.Models;

namespace VietGeeks.TestPlatform.TestManager.Core.ReadonlyModels;
public class AggregatedGrading
{
    public GradingCriteriaConfigType GradingType { get; set; }

    public bool? PassMark { get; set; }

    public Dictionary<string, string>? Grades { get; set; }
}