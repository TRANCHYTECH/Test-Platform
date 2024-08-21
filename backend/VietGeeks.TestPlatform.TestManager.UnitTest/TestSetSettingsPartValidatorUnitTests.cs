using VietGeeks.TestPlatform.TestManager.Data.Models;
using VietGeeks.TestPlatform.TestManager.Infrastructure.Validators.TestDefinition;

namespace VietGeeks.TestPlatform.TestManager.UnitTest;

[Collection(TestDefinitionValidatorTestCollection.CollectionId)]
public class TestSetSettingsPartValidatorUnitTests
{
    private readonly TestDefinitionValidatorFixture _fixture;
    private readonly TestSetSettingsPartValidator _validator;

    public TestSetSettingsPartValidatorUnitTests(TestDefinitionValidatorFixture fixture)
    {
        _fixture = fixture;
        _validator = _fixture.CreateTestSetSettingsPartValidator();
    }

    #region TestSetSettingsPart

    [Fact]
    public async Task TestSetSettingsPart_Validate_Failure_Null()
    {
        var input = new TestSetSettingsPart
        {
            Generator = default!,
            GeneratorType = 0
        };
        var result = await _validator.ValidateAsync(input);

        Assert.False(result.IsValid);
        Assert.Equal(1, result.Errors.Count(c => c.PropertyName == nameof(input.GeneratorType) && c.ErrorCode == "EnumValidator"));
        Assert.Equal(1, result.Errors.Count(c => c.PropertyName == nameof(input.Generator) && c.ErrorCode == "NotNullValidator"));
    }

    [Fact]
    public async Task TestSetSettingsPart_Validate_Failure_Required_RandomFromCategoriesGeneratorConfig()
    {
        var input = new TestSetSettingsPart
        {
            GeneratorType = TestSetGeneratorType.RandomByCategories,
            Generator = new RandomFromCategoriesGenerator()
            {
                Configs =
                [
                    new RandomFromCategoriesGeneratorConfig
                    {
                    }
                ]
            }
        };

        var result = await _validator.ValidateAsync(input);

        Assert.False(result.IsValid);
        Assert.Equal(1, result.Errors.Count(c => c.PropertyName.EndsWith(nameof(RandomFromCategoriesGeneratorConfig.QuestionCategoryId)) && c.ErrorCode == "NotEmptyValidator"));
        Assert.Equal(1, result.Errors.Count(c => c.PropertyName.EndsWith(nameof(RandomFromCategoriesGeneratorConfig.DrawNumber)) && c.ErrorCode == "GreaterThanOrEqualValidator"));
    }

    [Fact]
    public async Task TestSetSettingsPart_Validate_Failure_MismatchGeneratorType()
    {
        var input = new TestSetSettingsPart
        {
            GeneratorType = TestSetGeneratorType.Default,
            Generator = new RandomFromCategoriesGenerator()
        };

        var result = await _validator.ValidateAsync(input);

        Assert.False(result.IsValid);
        Assert.Equal(1, result.Errors.Count(c => c.PropertyName.EndsWith(nameof(TestSetSettingsPart.Generator)) && c.ErrorCode == "PredicateValidator"));
    }

    [Fact]
    public async Task TestSetSettingsPart_Validate_Success()
    {
        var input = new TestSetSettingsPart
        {
            GeneratorType = TestSetGeneratorType.RandomByCategories,
            Generator = new RandomFromCategoriesGenerator
            {
                Configs =
                [
                    new RandomFromCategoriesGeneratorConfig
                    {
                        QuestionCategoryId = "001",
                        DrawNumber = 1
                    }
                ]
            }
        };

        var result = await _validator.ValidateAsync(input);

        Assert.True(result.IsValid);
    }

    #endregion
}
