using FluentValidation;
using VietGeeks.TestPlatform.TestManager.Core.Models;
using VietGeeks.TestPlatform.TestManager.Infrastructure.Services;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure.Validators.TestDefintion;

// Use rule set for add,update, IRuleBuilderOptions
public class TestBasicSettingsPartValidator : AbstractValidator<TestBasicSettingsPart>
{
    public TestBasicSettingsPartValidator(ITestCategoryService testCategoryService)
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(c => c.Name)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(c => c.Description)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(c => c.Category)
            .NotEmpty()
            .MustAsync(async (value, ctx) => await testCategoryService.CheckTestCategoryExistence(value))
            .WithMessage("Invalid test category");
    }
}
