using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VietGeeks.TestPlatform.TestManager.Contract.ViewModels;


public class CreateOrupdateGradingSettings
{
    public TestEndConfig TestEndConfig { get; set; } = default!;

    public Dictionary<string, GradingCriteriaConfig> GradingCriterias { get; set; } = new Dictionary<string, GradingCriteriaConfig>();
}

public class GradingSettings
{
    public TestEndConfig TestEndConfig { get; set; } = default!;

    public Dictionary<string, GradingCriteriaConfig> GradingCriterias { get; set; } = new Dictionary<string, GradingCriteriaConfig>();
}

public class TestEndConfig
{
    public string? Message { get; set; }

    public bool RedirectTo { get; set; }

    public string? ToAddress { get; set; }
}

[JsonDerivedType(typeof(PassMaskCriteria), (int)GradingCriteriaConfigType.PassMask)]
[JsonDerivedType(typeof(GradeRangeCriteria), (int)GradingCriteriaConfigType.GradeRanges)]
public abstract class GradingCriteriaConfig
{
}

public class PassMaskCriteria : GradingCriteriaConfig
{
    public int Value { get; set; }

    public RangeUnit Unit { get; set; }
}

public class GradeRangeCriteria : GradingCriteriaConfig
{
    public GradeType GradeType { get; set; }

    public RangeUnit Unit { get; set; }

    public List<GradeRangeCriteriaDetail> Details { get; set; } = new List<GradeRangeCriteriaDetail>();
}

public class GradeRangeCriteriaDetail
{
    public int From { get; set; }

    public int To { get; set; }

    public Dictionary<string, string> Grades { get; set; } = new Dictionary<string, string>();
}

public enum GradingCriteriaConfigType
{
    PassMask = 1,
    GradeRanges = 2
}

public enum RangeUnit
{
    Percent = 1,
    Point = 2
}

public enum GradeType
{
    Grade = 1,
    Descriptive = 2,
    GradeAndDescriptive = 3
}

