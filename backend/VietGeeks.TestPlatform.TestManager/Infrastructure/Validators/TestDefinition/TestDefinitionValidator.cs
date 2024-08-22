using FluentValidation;
using FluentValidation.Results;
using VietGeeks.TestPlatform.TestManager.Data.Models;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure.Validators.TestDefinition
{
    public class TestDefinitionValidator : AbstractValidator<Data.Models.TestDefinition>
    {
        public static readonly string[] ChildValidators =
        [
            nameof(Data.Models.TestDefinition.BasicSettings),
            nameof(Data.Models.TestDefinition.TestSetSettings),
            nameof(Data.Models.TestDefinition.TestAccessSettings),
            nameof(Data.Models.TestDefinition.GradingSettings)
        ];

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

        protected override bool PreValidate(ValidationContext<Data.Models.TestDefinition> context,
            ValidationResult result)
        {
            context.RootContextData["TestId"] = context.InstanceToValidate.ID;
            return true;
        }
    }
}