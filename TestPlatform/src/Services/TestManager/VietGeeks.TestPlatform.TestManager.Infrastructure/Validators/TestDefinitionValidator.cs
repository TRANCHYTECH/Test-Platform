using FluentValidation;
using VietGeeks.TestPlatform.TestManager.Core.Models;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure.Validators;

public class TestDefinitionValidator : AbstractValidator<TestDefinition>
{
    public TestDefinitionValidator(
        IValidator<TestBasicSettingsPart> testBasicSettingsValidator,
        IValidator<TimeSettingsPart> timeSettingsValidator,
        IValidator<TestSetSettingsPart> testSetSettingsPartValidator)
    {
        RuleFor(c => c.BasicSettings).SetValidator(testBasicSettingsValidator);
        RuleFor(c => c.TimeSettings).SetValidator(timeSettingsValidator);
        RuleFor(c => c.TestSetSettings).SetValidator(testSetSettingsPartValidator);
    }
}
