using Faker;
using FluentValidation.TestHelper;
using VietGeeks.TestPlatform.TestManager.Data.Models;
using VietGeeks.TestPlatform.TestManager.Infrastructure.Validators.TestDefinition;

namespace VietGeeks.TestPlatform.TestManager.UnitTest;

[Collection(TestDefinitionValidatorTestCollection.CollectionId)]
public class TestBasicSettingsPartValidatorUnitTests(TestDefinitionValidatorFixture fixture)
{
    private readonly TestDefinitionValidatorFixture _fixture = fixture;

    private readonly TestBasicSettingsPartValidator _validator = fixture.CreateTestBasicSettingsPartValidator();


    #region TestBasicSettingsPart

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("yesme", true)]
    [InlineData("notme", false)]
    public async Task TestBasicSettingsPart_Validate_Failure_At_Category(string categoryId, bool isValidCategory)
    {
        var input = new TestBasicSettingsPart
        {
            Category = categoryId
        };
        var validateResult = await _validator.TestValidateAsync(input);

        Assert.False(validateResult.IsValid);
        Assert.Equal(isValidCategory,
            !validateResult.Errors.Exists(c => c.PropertyName == nameof(TestDefinition.BasicSettings.Category)));
    }

    [Fact]
    public async Task TestBasicSettingsPart_Validate_Failure_NotEmpty()
    {
        var input = new TestBasicSettingsPart();
        var validateResult = await _validator.TestValidateAsync(input);

        Assert.False(validateResult.IsValid);
        Assert.Equal(2, validateResult.Errors.Count(c => c.ErrorCode == "NotEmptyValidator"));
    }

    [Fact]
    public async Task TestBasicSettingsPart_Validate_Failure_MaximumLength()
    {
        var input = new TestBasicSettingsPart
        {
            Name = Lorem.Sentence(501),
            Description = Lorem.Sentence(1001),
            Category = "yesme"
        };
        var result = await _validator.TestValidateAsync(input);

        Assert.False(result.IsValid);
        Assert.Equal(2, result.Errors.Count(c => c.ErrorCode == "MaximumLengthValidator"));
    }

    [Fact]
    public async Task TestBasicSettingsPart_Validate_Success()
    {
        var input = new TestBasicSettingsPart
        {
            Name = Lorem.Sentence(2),
            Description = Lorem.Sentence(4),
            Category = "yesme"
        };
        var result = await _validator.TestValidateAsync(input);

        Assert.True(result.IsValid);
    }

    #endregion
}