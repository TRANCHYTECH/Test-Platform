using FluentValidation;
using FluentValidation.Results;
using VietGeeks.TestPlatform.TestManager.Core.Models;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure.Validators;

public class TestDefinitionValidator : AbstractValidator<TestDefinition>
{
    public TestDefinitionValidator(
        IValidator<TestBasicSettingsPart> testBasicSettingsValidator,
        IValidator<TestSetSettingsPart> testSetSettingsPartValidator,
        IValidator<TestAccessSettingsPart> testAccessSettingsPartValidator,
        IValidator<GradingSettingsPart> gradingSettingsPartValidator)
    {
        RuleFor(c => c.BasicSettings).NotNull().SetValidator(testBasicSettingsValidator);
        RuleFor(c => c.TestSetSettings).NotNull().SetValidator(testSetSettingsPartValidator);
        RuleFor(c => c.TestAccessSettings).NotNull().SetValidator(testAccessSettingsPartValidator);
        RuleFor(c => c.GradingSettings).NotNull().SetValidator(gradingSettingsPartValidator);
    }

    protected override bool PreValidate(ValidationContext<TestDefinition> context, ValidationResult result)
    {
        context.RootContextData["TestId"] = context.InstanceToValidate.ID;
        return true;
    }

    public static readonly string[] ChildValidators = new string[] {
        nameof(TestDefinition.BasicSettings),
        nameof(TestDefinition.TestSetSettings),
        nameof(TestDefinition.TestAccessSettings),
        nameof(TestDefinition.GradingSettings)
    };
}