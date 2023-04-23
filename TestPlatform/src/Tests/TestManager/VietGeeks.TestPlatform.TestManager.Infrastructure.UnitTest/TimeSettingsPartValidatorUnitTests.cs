using VietGeeks.TestPlatform.TestManager.Core.Models;
using VietGeeks.TestPlatform.TestManager.Infrastructure.Validators.TestDefintion;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure.UnitTest;

[Collection("TestDefinitionValidatorTestCollection")]
public class TimeSettingsPartValidatorUnitTests
{
    private readonly TestDefinitionValidatorFixture _fixture;
    private readonly TimeSettingsPartValidator _validator;

    public TimeSettingsPartValidatorUnitTests(TestDefinitionValidatorFixture fixture)
    {
        _fixture = fixture;
        _validator = fixture.CreateTimeSettingsPartValidator();
    }

    [Theory]
    [MemberData(nameof(GetTestData_TestDurationMethod_Failure))]
    public async Task TimeSettingsPart_TestDurationMethod_Validate_Failure(TestDurationMethod input)
    {
        var testedPart = TimeSettingsPart.Default();
        testedPart.TestDurationMethod = input;

        var result = await _validator.ValidateAsync(testedPart);

        Assert.False(result.IsValid);
        Assert.Equal(1, result.Errors.Count(c => c.PropertyName == "TestDurationMethod.Duration"));
    }


    [Theory]
    [MemberData(nameof(GetTestData_TestActivationMethod_Failure))]
    public async Task TimeSettingsPart_TestActivationMethod_Validate_Failure(TestActivationMethod input)
    {
        var testedPart = TimeSettingsPart.Default();
        testedPart.TestActivationMethod= input;

        var result = await _validator.ValidateAsync(testedPart);

        Assert.False(result.IsValid);
        Assert.Equal(1, result.Errors.Count(c => c.PropertyName.StartsWith(nameof(TimeSettingsPart.TestActivationMethod))));
    }

    [Theory]
    [MemberData(nameof(GetTestData_TestDurationMethod_Success))]
    public async Task TimeSettingsPart_TestDurationMethod_Validate_Success(TestDurationMethod input)
    {
        var testedPart = TimeSettingsPart.Default();
        testedPart.TestDurationMethod = input;

        var result = await _validator.ValidateAsync(testedPart);

        Assert.True(result.IsValid);
    }

    [Theory]
    [MemberData(nameof(GetTestData_TestActivationMethod_Success))]
    public async Task TimeSettingsPart_TestActivationMethod_Validate_Success(TestActivationMethod input)
    {
        var testedPart = TimeSettingsPart.Default();
        testedPart.TestActivationMethod = input;

        var result = await _validator.ValidateAsync(testedPart);

        Assert.True(result.IsValid);
    }

    public static IEnumerable<object[]> GetTestData_TestActivationMethod_Failure()
    {
        var wrong1 = new ManualTestActivation
        {
            ActiveUntil = TimeSpan.Parse("00:00:00")
        };

        var wrong2 = new ManualTestActivation
        {
            ActiveUntil = TimeSpan.FromDays(100)
        };

        var wrong3 = new TimePeriodActivation
        {
            ActiveFromDate = DateTime.UtcNow.AddMinutes(-10),
            ActiveUntilDate = DateTime.UtcNow.AddMinutes(-5)
        };

        var wrong4 = new TimePeriodActivation
        {
            ActiveFromDate = DateTime.UtcNow.AddMinutes(2),
            ActiveUntilDate = DateTime.UtcNow.AddMinutes(1)
        };

        return new List<object[]>
        {
            new object[] { wrong1 },
            new object[] { wrong2 },
            new object[] { wrong3 },
            new object[] { wrong4 }
        };
    }

    public static IEnumerable<object[]> GetTestData_TestDurationMethod_Failure()
    {
        var wrong0 = new CompleteQuestionDuration();

        var wrong1 = new CompleteQuestionDuration
        {
            Duration = TimeSpan.Parse("00:00:00")
        };
         
        var wrong2 = new CompleteQuestionDuration
        {
            Duration = TimeSpan.Parse("01:00:00")
        };

        var wrong3 = new CompleteTestDuration
        {
            Duration = TimeSpan.Parse("00:00:00")
        };

        var wrong4 = new CompleteTestDuration
        {
            Duration = TimeSpan.Parse("1.00:00:00")
        };

        return new List<object[]>
        {
            new object[] { wrong0 },
            new object[] { wrong1 },
            new object[] { wrong2 },
            new object[] { wrong3 },
            new object[] { wrong4 }
        };
    }

    public static IEnumerable<object[]> GetTestData_TestDurationMethod_Success()
    {
        var wrong1 = new CompleteQuestionDuration
        {
            Duration = TimeSpan.Parse("00:00:01")
        };

        var wrong2 = new CompleteQuestionDuration
        {
            Duration = TimeSpan.Parse("00:59:59")
        };

        var wrong3 = new CompleteTestDuration
        {
            Duration = TimeSpan.Parse("00:00:01")
        };

        var wrong4 = new CompleteTestDuration
        {
            Duration = TimeSpan.Parse("23:59:59")
        };

        return new List<object[]>
        {
            new object[] { wrong1 },
            new object[] { wrong2 },
            new object[] { wrong3 },
            new object[] { wrong4 }
        };
    }

    public static IEnumerable<object[]> GetTestData_TestActivationMethod_Success()
    {
        var right1 = new ManualTestActivation
        {
            ActiveUntil = TimeSpan.Parse("00:00:01")
        };

        var right2 = new ManualTestActivation
        {
            ActiveUntil = TimeSpan.Parse("99.23:59:59")
        };

        var right3 = new TimePeriodActivation
        {
            ActiveFromDate = DateTime.UtcNow.AddSeconds(10),
            ActiveUntilDate = DateTime.UtcNow.AddMinutes(1)
        };


        return new List<object[]>
        {
            new object[] { right1 },
            new object[] { right2 },
            new object[] { right3 }
        };
    }
}
