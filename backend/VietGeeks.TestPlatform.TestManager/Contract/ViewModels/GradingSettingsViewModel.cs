﻿using System.Text.Json.Serialization;

namespace VietGeeks.TestPlatform.TestManager.Contract.ViewModels;

public class CreateOrUpdateGradingSettings
{
    public TestEndConfig TestEndConfig { get; set; } = default!;

    public Dictionary<string, GradingCriteriaConfig> GradingCriterias { get; set; } = new();

    public InformRespondentConfig InformRespondentConfig { get; set; } = default!;
}

public class GradingSettings
{
    public TestEndConfig TestEndConfig { get; set; } = default!;

    public Dictionary<string, GradingCriteriaConfig> GradingCriterias { get; set; } = new();

    public InformRespondentConfig InformRespondentConfig { get; set; } = default!;
}

public class TestEndConfig
{
    public string? Message { get; set; }

    public bool RedirectTo { get; set; }

    public string? ToAddress { get; set; }
}

public class InformRespondentConfig
{
    public bool InformViaEmail { get; set; }

    public string? PassedMessage { get; set; }

    public string? FailedMessage { get; set; }

    public Dictionary<string, bool> InformFactors { get; set; } = new();
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

    public List<GradeRangeCriteriaDetail> Details { get; set; } = [];
}

public class GradeRangeCriteriaDetail
{
    public int To { get; set; }

    public Dictionary<string, string> Grades { get; set; } = new();
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

public enum InformFactor
{
    PercentageScore = 1,
    PointsScore = 2,
    Grade = 3,
    DescriptiveGrade = 4,
    CorrectAnwsers = 5,
    PassOrFailMessage = 6
}