using FluentValidation;
using FluentValidation.TestHelper;
using FluentValidation.Validators;
using Moq;
using VietGeeks.TestPlatform.TestManager.Core.Models;
using VietGeeks.TestPlatform.TestManager.Infrastructure.Services;
using VietGeeks.TestPlatform.TestManager.Infrastructure.Validators;
using Xunit.Sdk;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure.UnitTest;

public class TestDefinitionValidatorUnitTests
{
    private Mock<ITestCategoryService> _testCategoryServiceMock = new();

    public TestDefinitionValidatorUnitTests()
    {
        _testCategoryServiceMock.Setup(c => c.CheckTestCategoryExistence(It.IsAny<string>())).ReturnsAsync((string arg) => arg == "yesme" ? true : false);
    }

    #region TestBasicSettingsPart

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("yesme", true)]
    [InlineData("notme", false)]
    public async Task TestBasicSettingsPart_Validate_Failure_At_Category(string categoryId, bool isValidCategory)
    {
        var input = new TestBasicSettingsPart()
        {
            Category = categoryId
        };
        var validator = new TestBasicSettingsPartValidator(_testCategoryServiceMock.Object);
        var validateResult = await validator.TestValidateAsync(input);

        Assert.False(validateResult.IsValid);
        Assert.Equal(isValidCategory, !validateResult.Errors.Exists(c => c.PropertyName == nameof(TestDefinition.BasicSettings.Category)));
    }

    [Fact]
    public async Task TestBasicSettingsPart_Validate_Failure_NotEmpty()
    {
        var input = new TestBasicSettingsPart();
        var validator = new TestBasicSettingsPartValidator(_testCategoryServiceMock.Object);
        var validateResult = await validator.TestValidateAsync(input);

        Assert.False(validateResult.IsValid);
        Assert.Equal(3, validateResult.Errors.Count(c => c.ErrorCode == "NotEmptyValidator"));
    }

    [Fact]
    public async Task TestBasicSettingsPart_Validate_Failure_MaximumLength()
    {
        var input = new TestBasicSettingsPart
        {
            Name = Faker.Lorem.Sentence(501),
            Description = Faker.Lorem.Sentence(1001),
            Category = "yesme"
        };
        var validator = new TestBasicSettingsPartValidator(_testCategoryServiceMock.Object);
        var result = await validator.TestValidateAsync(input);

        Assert.False(result.IsValid);
        Assert.Equal(2, result.Errors.Count(c => c.ErrorCode == "MaximumLengthValidator"));
    }

    [Fact]
    public async Task TestBasicSettingsPart_Validate_Success()
    {
        var input = new TestBasicSettingsPart
        {
            Name = Faker.Lorem.Sentence(2),
            Description = Faker.Lorem.Sentence(4),
            Category = "yesme"
        };
        var validator = new TestBasicSettingsPartValidator(_testCategoryServiceMock.Object);
        var result = await validator.TestValidateAsync(input);

        Assert.True(result.IsValid);
    }

    #endregion

    #region TestSetSettingsPart

    [Fact]
    public async Task TestSetSettingsPart_Validate_Failure_Null()
    {
        var input = new TestSetSettingsPart
        {
            Generator = default!
        };
        var validator = createTestSetSettingsPartValidator();
        var result = await validator.ValidateAsync(input);

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
                Configs = new List<RandomFromCategoriesGeneratorConfig>
                {
                    new RandomFromCategoriesGeneratorConfig
                    {
                    }
                }
            }
        };

        var validator = createTestSetSettingsPartValidator();
        var result = await validator.ValidateAsync(input);

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

        var validator = createTestSetSettingsPartValidator();
        var result = await validator.ValidateAsync(input);

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
                Configs = new List<RandomFromCategoriesGeneratorConfig>
                {
                    new RandomFromCategoriesGeneratorConfig
                    {
                        QuestionCategoryId = "001",
                        DrawNumber = 1
                    }
                }
            }
        };

        var validator = createTestSetSettingsPartValidator();
        var result = await validator.ValidateAsync(input);

        Assert.True(result.IsValid);
    }

    private TestSetSettingsPartValidator createTestSetSettingsPartValidator()
    {
        return new TestSetSettingsPartValidator(new DefaultGeneratorValidator(), new RandomFromCategoriesGeneratorValidator(new RandomFromCategoriesGeneratorConfigValidator()));
    }

    #endregion
}