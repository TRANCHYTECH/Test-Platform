using FluentValidation;
using FluentValidation.TestHelper;
using VietGeeks.TestPlatform.TestManager.Data.Models;
using VietGeeks.TestPlatform.TestManager.Infrastructure.Validators.TestDefinition;

namespace VietGeeks.TestPlatform.TestManager.UnitTest;

[Collection(TestDefinitionValidatorTestCollection.CollectionId)]
public class TestDefinitionValidatorUnitTests(TestDefinitionValidatorFixture fixture)
{
    /// <summary>
    /// Create a wrong input to verify all child validators are envolved.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task TestDefinition_Validator_Ensure_AllChildValidatorsWorks_Success()
    {
        var input = new TestDefinition
        {
            BasicSettings = new TestBasicSettingsPart(),
            TestSetSettings = new TestSetSettingsPart { GeneratorType = 0 },
            TestAccessSettings = new TestAccessSettingsPart(),
            GradingSettings = new GradingSettingsPart()
        };
        var ctx = new ValidationContext<TestDefinition>(input);
        var validator = fixture.CreateTestDefinitionValidator();
        var result = await validator.TestValidateAsync(ctx);
        Assert.False(result.IsValid);
        Assert.True(TestDefinitionValidator.ChildValidators.ToList().TrueForAll(c =>
        {
            return result.Errors.Any(err => err.PropertyName.StartsWith(c));
        }));
    }
}
