using MongoDB.Bson.Serialization.Attributes;
using VietGeeks.TestPlatform.SharedKernel;

namespace VietGeeks.TestPlatform.TestManager.Data.Models;

public class GradingSettingsPart
{
    public required TestEndConfig TestEndConfig { get; init; }

    public required IDictionary<string, GradingCriteriaConfig> GradingCriterias { get; init; }

    public required InformRespondentConfig InformRespondentConfig { get; init; }

    public static GradingSettingsPart Default()
    {
        GradingSettingsPart defaultSetting = new()
        {
            TestEndConfig = new TestEndConfig
            {
                //todo: move to resource like lookup db.
                Message = "Thank you for taking the test!",
            },
            InformRespondentConfig = new InformRespondentConfig(),
            GradingCriterias = new Dictionary<string, GradingCriteriaConfig>(),
        };

        defaultSetting.GradingCriterias.Add(GradingCriteriaConfigType.PassMask.Value(), new PassMaskCriteria
        {
            Unit = RangeUnit.Percent,
            Value = 50,
        });

        return defaultSetting;
    }
}

public class TestEndConfig
{
    public string? Message { get; init; }

    public bool RedirectTo { get; init; }

    public string? ToAddress { get; init; }
}

public class InformRespondentConfig
{
    public bool InformViaEmail { get; init; }

    public string? PassedMessage { get; init; }

    public string? FailedMessage { get; init; }

    public IDictionary<string, bool> InformFactors { get; init; } = new Dictionary<string, bool>();
}

public enum InformFactor
{
    PercentageScore = 1,
    PointsScore = 2,
    Grade = 3,
    DescriptiveGrade = 4,
    CorrectAnswers = 5,
    PassOrFailMessage = 6,
}

[BsonKnownTypes(typeof(PassMaskCriteria), typeof(GradeRangeCriteria))]
public abstract class GradingCriteriaConfig;

public class PassMaskCriteria : GradingCriteriaConfig
{
    public int Value { get; init; }

    public RangeUnit Unit { get; init; }
}

public class GradeRangeCriteria : GradingCriteriaConfig
{
    public GradeType GradeType { get; init; }

    public RangeUnit Unit { get; init; }

    public IList<GradeRangeCriteriaDetail> Details { get; init; } = [];
}

public class GradeRangeCriteriaDetail
{
    public int To { get; init; }

    public IDictionary<string, string> Grades { get; init; } = new Dictionary<string, string>();
}

public enum GradingCriteriaConfigType
{
    PassMask = 1,
    GradeRanges = 2,
}

public enum RangeUnit
{
    Percent = 1,
    Point = 2,
}

public enum GradeType
{
    Grade = 1,
    Descriptive = 2,
    GradeAndDescriptive = 3,
}