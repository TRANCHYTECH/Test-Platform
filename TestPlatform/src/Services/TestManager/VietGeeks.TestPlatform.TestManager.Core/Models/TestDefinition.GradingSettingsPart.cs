using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace VietGeeks.TestPlatform.TestManager.Core.Models;

public class GradingSettingsPart
{
    public TestEndConfig TestEndConfig { get; set; } = default!;

    public Dictionary<string, GradingCriteriaConfig> GradingCriterias { get; set; } = new Dictionary<string, GradingCriteriaConfig>();

    public InformRespondentConfig InformRespondentConfig { get; set; } = default!;

    public static GradingSettingsPart Default()
    {
        GradingSettingsPart defaultSetting = new()
        {
            TestEndConfig = new()
            {
                //todo: move to resource like lookup db.
                Message = "Thank you for taking the test!"
            },
            InformRespondentConfig = new() {
            }
        };

        defaultSetting.GradingCriterias.Add(GradingCriteriaConfigType.PassMask.Value(), new PassMaskCriteria
        {
            Unit = RangeUnit.Percent,
            Value = 50
        });

        return defaultSetting;
    }
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

    public Dictionary<string, bool> InformFactors { get; set; } = new Dictionary<string, bool>();
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

[BsonKnownTypes(typeof(PassMaskCriteria), typeof(GradeRangeCriteria))]
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