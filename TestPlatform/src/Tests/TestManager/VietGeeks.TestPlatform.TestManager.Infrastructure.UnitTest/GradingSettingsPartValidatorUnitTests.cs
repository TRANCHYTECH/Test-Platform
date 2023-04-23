using Amazon.Runtime.Internal.Transform;
using FluentValidation;
using FluentValidation.TestHelper;
using FluentValidation.Validators;
using VietGeeks.TestPlatform.TestManager.Core.Models;
using VietGeeks.TestPlatform.TestManager.Infrastructure.Validators.TestDefintion;
using Xunit.Sdk;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure.UnitTest;

[Collection("TestDefinitionValidatorTestCollection")]
public class GradingSettingsPartValidatorUnitTests
{
    private readonly TestDefinitionValidatorFixture _fixture;

    public GradingSettingsPartValidatorUnitTests(TestDefinitionValidatorFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GradingSettingsPart_TestEndConfig_Validate_Failure_Miss_ToAddress()
    {
        var input = new TestEndConfig
        {
            Message = Faker.Lorem.Sentence(10),
            RedirectTo = true
        };

        var validator = new TestEndConfigValidator();

        var result = await validator.TestValidateAsync(input);
        Assert.False(result.IsValid);
        Assert.Equal(1, result.Errors.Count(c => c.PropertyName.EndsWith(nameof(TestEndConfig.ToAddress)) && c.ErrorCode == "NotEmptyValidator"));
    }

    [Fact]
    public async Task GradingSettingsPart_TestEndConfig_Validate_Failure_NoNeed_ToAddress()
    {
        var input = new TestEndConfig
        {
            Message = Faker.Lorem.Sentence(10),
            RedirectTo = false,
            ToAddress = "http://testmaster.io"
        };

        var validator = new TestEndConfigValidator();

        var result = await validator.TestValidateAsync(input);
        Assert.False(result.IsValid);
        Assert.Equal(1, result.Errors.Count(c => c.PropertyName.EndsWith(nameof(TestEndConfig.ToAddress)) && c.ErrorCode == "NullValidator"));
    }

    [Fact]
    public async Task GradingSettingsPart_TestEndConfig_Validate_Success()
    {
        var input = new TestEndConfig
        {
            Message = Faker.Lorem.Sentence(10),
            RedirectTo = true,
            ToAddress = "http://testmaster.io"
        };

        var validator = new TestEndConfigValidator();

        var result = await validator.TestValidateAsync(input);
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(101)]
    public async Task GradingSettingsPart_PassMaskCriteria_Validate_Failure_InvalidPercentValue(int value)
    {
        var input = new PassMaskCriteria
        {
            Value = value,
            Unit = RangeUnit.Percent
        };

        var validator = new PassMaskCriteriaValidator();
        var result = await validator.TestValidateAsync(input);
        Assert.False(result.IsValid);
        Assert.Equal(1, result.Errors.Count(c => c.PropertyName == nameof(PassMaskCriteria.Value) && c.ErrorCode == "InclusiveBetweenValidator"));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task GradingSettingsPart_PassMaskCriteria_Validate_Failure_InvalidPointValue(int value)
    {
        var input = new PassMaskCriteria
        {
            Value = value,
            Unit = RangeUnit.Point
        };

        var validator = new PassMaskCriteriaValidator();
        var result = await validator.TestValidateAsync(input);
        Assert.False(result.IsValid);
        Assert.Equal(1, result.Errors.Count(c => c.PropertyName == nameof(PassMaskCriteria.Value) && c.ErrorCode == "GreaterThanValidator"));
    }

    [Theory]
    [InlineData(1, RangeUnit.Percent)]
    [InlineData(100, RangeUnit.Percent)]
    [InlineData(1, RangeUnit.Point)]
    [InlineData(100, RangeUnit.Point)]
    public async Task GradingSettingsPart_PassMaskCriteria_Validate_Success(int value, RangeUnit unit)
    {
        var input = new PassMaskCriteria
        {
            Value = value,
            Unit = unit
        };

        var validator = new PassMaskCriteriaValidator();
        var result = await validator.TestValidateAsync(input);
        Assert.True(result.IsValid);
    }

    [Theory]
    [MemberData(nameof(GetTestData_Case_ERR_001))]
    public async Task GradingSettingsPart_GradeRangeCriteria_Validate_Failure_ERR_001(GradeRangeCriteria input)
    {
        var result = await DoGradeRangeCriteriaValidate(input);
        Assert.False(result.IsValid);
        Assert.Equal(1, result.Errors.Count(c => c.ErrorMessage == "ERR.TESTDEF.GRADE.001"));
    }

    [Theory]
    [MemberData(nameof(GetTestData_Case_ERR_002))]
    public async Task GradingSettingsPart_GradeRangeCriteria_Validate_Failure_ERR_002(GradeRangeCriteria input)
    {
        var result = await DoGradeRangeCriteriaValidate(input);
        Assert.False(result.IsValid);
        Assert.Equal(1, result.Errors.Count(c => c.ErrorMessage == "ERR.TESTDEF.GRADE.002"));
    }

    [Theory]
    [MemberData(nameof(GetTestData_Case_ERR_003))]
    public async Task GradingSettingsPart_GradeRangeCriteria_Validate_Failure_ERR_003(GradeRangeCriteria input)
    {
        var result = await DoGradeRangeCriteriaValidate(input);
        Assert.False(result.IsValid);
        Assert.Equal(1, result.Errors.Count(c => c.ErrorMessage == "ERR.TESTDEF.GRADE.003"));
    }

    [Theory]
    [MemberData(nameof(GetTestData_Case_ERR_004))]
    public async Task GradingSettingsPart_GradeRangeCriteria_Validate_Failure_ERR_004(GradeRangeCriteria input)
    {
        var result = await DoGradeRangeCriteriaValidate(input);
        Assert.False(result.IsValid);
        Assert.Equal(1, result.Errors.Count(c => c.ErrorMessage == "ERR.TESTDEF.GRADE.004"));
    }

    [Theory]
    [MemberData(nameof(GetTestData_Success))]
    public async Task GradingSettingsPart_GradeRangeCriteria_Validate_Success(GradeRangeCriteria input)
    {
        var result = await DoGradeRangeCriteriaValidate(input);
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task GradingSettingsPart_Validate_Success()
    {
        //todo: rename.
        var input = CreateFullValidSampleOfGradingSettingsPart();

        var validator = _fixture.CreateGradingSettingsPartValidator();
        var ctx = new ValidationContext<GradingSettingsPart>(input);
        ctx.RootContextData["TestId"] = TestDefinitionValidatorFixture.TestMock.Id;
        var result = await validator.TestValidateAsync(ctx);

        Assert.True(result.IsValid);
    }

    public async Task<TestValidationResult<GradeRangeCriteria>> DoGradeRangeCriteriaValidate(GradeRangeCriteria input)
    {
        var validator = new GradeRangeCriteriaValidator(_fixture.QuestionManagerServiceMock.Object);
        var ctx = new ValidationContext<GradeRangeCriteria>(input);
        ctx.RootContextData["TestId"] = TestDefinitionValidatorFixture.TestMock.Id;

        return await validator.TestValidateAsync(ctx);
    }

    public static GradingSettingsPart CreateFullValidSampleOfGradingSettingsPart()
    {
        return new GradingSettingsPart
        {
            TestEndConfig = new TestEndConfig
            {
                Message = "some msgs"
            },
            GradingCriterias = new Dictionary<string, GradingCriteriaConfig>
            {
                { "1", new PassMaskCriteria {
                    Unit = RangeUnit.Percent,
                    Value = 50
                }
                },
                { "2", new GradeRangeCriteria {
                    Unit = RangeUnit.Point,
                    GradeType = GradeType.Grade,
                    Details = new List<GradeRangeCriteriaDetail>
                    {
                        new GradeRangeCriteriaDetail
                        {
                            To = TestDefinitionValidatorFixture.TestMock.TotalPoints,
                            Grades = new Dictionary<string, string>()
                            {
                                { "1","grade" }
                            }
                        }
                    }
                }
                }
            }
        };
    }

    public static IEnumerable<object[]> GetTestData_Case_ERR_001()
    {
        var wrong1 = new GradeRangeCriteria
        {
            GradeType = GradeType.Grade,
            Unit = RangeUnit.Percent,
            Details = new List<GradeRangeCriteriaDetail>
            {
                new GradeRangeCriteriaDetail
                {
                    To = 10,
                    Grades = new Dictionary<string, string>
                    {
                        {"1","Good" },
                        {"2","Good desc" }
                    }
                },
                new GradeRangeCriteriaDetail
                {
                    To = 10,
                    Grades = new Dictionary<string, string>
                    {
                        {"1","Good" },
                        {"2","Good desc" }
                    }
                }
            }
        };

        var wrong2 = new GradeRangeCriteria
        {
            GradeType = GradeType.Grade,
            Unit = RangeUnit.Point,
            Details = new List<GradeRangeCriteriaDetail>
            {
                new GradeRangeCriteriaDetail
                {
                    To = 10,
                    Grades = new Dictionary<string, string>
                    {
                        {"1","Good" },
                        {"2","Good desc" }
                    }
                },
                new GradeRangeCriteriaDetail
                {
                    To = 10,
                    Grades = new Dictionary<string, string>
                    {
                        {"1","Good" },
                        {"2","Good desc" }
                    }
                }
            }
        };

        return new List<object[]>
        {
            new object[] { wrong1 },
            new object[] { wrong2 }
        };
    }

    public static IEnumerable<object[]> GetTestData_Case_ERR_002()
    {
        var wrong1 = new GradeRangeCriteria
        {
            GradeType = GradeType.Grade,
            Unit = RangeUnit.Percent,
            Details = new List<GradeRangeCriteriaDetail>
            {
                new GradeRangeCriteriaDetail
                {
                    To = 0,
                    Grades = new Dictionary<string, string>
                    {
                        { "1", "Good" },
                        { "2", "Good desc" }
                    }
                },
                new GradeRangeCriteriaDetail
                {
                    To = 100,
                    Grades = new Dictionary<string, string>
                    {
                        { "1", "Good" },
                        { "2", "Good desc" }
                    }
                }
            }
        };

        var wrong2 = new GradeRangeCriteria
        {
            GradeType = GradeType.GradeAndDescriptive,
            Unit = RangeUnit.Point,
            Details = new List<GradeRangeCriteriaDetail>
            {
                new GradeRangeCriteriaDetail
                {
                    To = 0,
                    Grades = new Dictionary<string, string>
                    {
                        { "1", "Good" },
                        { "2", "Good desc" }
                    }
                },
                new GradeRangeCriteriaDetail
                {
                    To = 100,
                    Grades = new Dictionary<string, string>
                    {
                        { "1", "Good" },
                        { "2", "Good desc" }
                    }
                }
            }
        };

        return new List<object[]>
        {
            new object[] { wrong1 },
            new object[] { wrong2 }
        };
    }

    public static IEnumerable<object[]> GetTestData_Case_ERR_003()
    {
        var wrong1 = new GradeRangeCriteria
        {
            GradeType = GradeType.Grade,
            Unit = RangeUnit.Percent,
            Details = new List<GradeRangeCriteriaDetail>
            {
                new GradeRangeCriteriaDetail
                {
                    To = 10,
                    Grades = new Dictionary<string, string>
                    {
                        {"1","Good" },
                        {"2","Good desc" }
                    }
                },
                new GradeRangeCriteriaDetail
                {
                    To = 101,
                    Grades = new Dictionary<string, string>
                    {
                        {"1","Good" },
                        {"2","Good desc" }
                    }
                }
            }
        };

        var wrong2 = new GradeRangeCriteria
        {
            GradeType = GradeType.Grade,
            Unit = RangeUnit.Percent,
            Details = new List<GradeRangeCriteriaDetail>
            {
                new GradeRangeCriteriaDetail
                {
                    To = 101,
                    Grades = new Dictionary<string, string>
                    {
                        {"1","Good" },
                        {"2","Good desc" }
                    }
                }
            }
        };

        var wrong3 = new GradeRangeCriteria
        {
            GradeType = GradeType.Grade,
            Unit = RangeUnit.Point,
            Details = new List<GradeRangeCriteriaDetail>
            {
                new GradeRangeCriteriaDetail
                {
                    To = TestDefinitionValidatorFixture.TestMock.TotalPoints + 10,
                    Grades = new Dictionary<string, string>
                    {
                        {"1","Good" },
                        {"2","Good desc" }
                    }
                }
            }
        };

        return new List<object[]>
        {
            new object[] { wrong1 },
            new object[] { wrong2 },
            new object[] { wrong3 }
        };
    }

    public static IEnumerable<object[]> GetTestData_Case_ERR_004()
    {
        var wrong1 = new GradeRangeCriteria
        {
            GradeType = GradeType.Grade,
            Unit = RangeUnit.Percent,
            Details = new List<GradeRangeCriteriaDetail>
            {
                new GradeRangeCriteriaDetail
                {
                    To = 100,
                    Grades = new Dictionary<string, string>
                    {
                        {"2","Descriptive" }
                    }
                }
            }
        };

        var wrong2 = new GradeRangeCriteria
        {
            GradeType = GradeType.Descriptive,
            Unit = RangeUnit.Percent,
            Details = new List<GradeRangeCriteriaDetail>
            {
                new GradeRangeCriteriaDetail
                {
                    To = 100,
                    Grades = new Dictionary<string, string>
                    {
                        {"1","Grade" },
                        {"3","Unknown" }
                    }
                }
            }
        };

        var wrong3 = new GradeRangeCriteria
        {
            GradeType = GradeType.GradeAndDescriptive,
            Unit = RangeUnit.Percent,
            Details = new List<GradeRangeCriteriaDetail>
            {
                new GradeRangeCriteriaDetail
                {
                    To = 50,
                    Grades = new Dictionary<string, string>
                    {
                        {"1","Grade" },
                        {"2","Descriptive" }
                    }
                },
                new GradeRangeCriteriaDetail
                {
                    To = 100,
                    Grades = new Dictionary<string, string>
                    {
                        {"1","Grade" },
                        {"3","Unknown" }
                    }
                }
            }
        };
        return new List<object[]>
        {
            new object[] { wrong1 },
            new object[] { wrong2 },
            new object[] { wrong3 }
        };
    }

    public static IEnumerable<object[]> GetTestData_Success()
    {
        var right1 = new GradeRangeCriteria
        {
            GradeType = GradeType.GradeAndDescriptive,
            Unit = RangeUnit.Percent,
            Details = new List<GradeRangeCriteriaDetail>
            {
                new GradeRangeCriteriaDetail
                {
                    To = 50,
                    Grades = new Dictionary<string, string>
                    {
                        {"1","Grade" },
                        {"2","Descriptive" }
                    }
                },
                new GradeRangeCriteriaDetail
                {
                    To = 100,
                    Grades = new Dictionary<string, string>
                    {
                        {"1","Grade" },
                        {"2","Descriptive" }
                    }
                }
            }
        };

        var right2 = new GradeRangeCriteria
        {
            GradeType = GradeType.GradeAndDescriptive,
            Unit = RangeUnit.Point,
            Details = new List<GradeRangeCriteriaDetail>
            {
                new GradeRangeCriteriaDetail
                {
                    To = 10,
                    Grades = new Dictionary<string, string>
                    {
                        {"1","Grade" },
                        {"2","Descriptive" }
                    }
                },
                new GradeRangeCriteriaDetail
                {
                    To =  TestDefinitionValidatorFixture.TestMock.TotalPoints, // matches total points returned from according mock service.
                    Grades = new Dictionary<string, string>
                    {
                        {"1","Grade" },
                        {"2","Descriptive" }
                    }
                }
            }
        };

        return new List<object[]>
        {
            new object[] { right1 },
            new object[] { right2 }
        };
    }
}