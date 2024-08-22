using FluentValidation;
using VietGeeks.TestPlatform.TestManager.Data.Models;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure.Validators.TestDefinition
{
    public class TestSetSettingsPartValidator : AbstractValidator<TestSetSettingsPart>
    {
        public TestSetSettingsPartValidator(IValidator<DefaultGenerator> defaultGeneratorValidator,
            IValidator<RandomFromCategoriesGenerator> randomFromCategoriesGeneratorValidator)
        {
            RuleFor(c => c.GeneratorType).IsInEnum();
            RuleFor(c => c.Generator).NotNull().Must(MatchGeneratorTypeCheck()).WithMessage("Generator Type Mismatch")
                .SetInheritanceValidator(v =>
                {
                    v.Add(defaultGeneratorValidator);
                    v.Add(randomFromCategoriesGeneratorValidator);
                });
        }

        private static Func<TestSetSettingsPart, TestSetGenerator, bool> MatchGeneratorTypeCheck()
        {
            return (part, prop) =>
            {
                switch (part.GeneratorType)
                {
                    case TestSetGeneratorType.Default:
                        return prop is DefaultGenerator;
                    case TestSetGeneratorType.RandomByCategories:
                        return prop is RandomFromCategoriesGenerator;
                    default:
                        return false;
                }
            };
        }
    }

    public class DefaultGeneratorValidator : AbstractValidator<DefaultGenerator>
    {
    }

    public class RandomFromCategoriesGeneratorValidator : AbstractValidator<RandomFromCategoriesGenerator>
    {
        public RandomFromCategoriesGeneratorValidator(IValidator<RandomFromCategoriesGeneratorConfig> validator1)
        {
            //todo: validate category id is valid.
            RuleForEach(c => c.Configs).NotNull().SetValidator(validator1);
        }
    }

    public class RandomFromCategoriesGeneratorConfigValidator : AbstractValidator<RandomFromCategoriesGeneratorConfig>
    {
        public RandomFromCategoriesGeneratorConfigValidator()
        {
            RuleFor(c => c.QuestionCategoryId).NotEmpty();
            RuleFor(c => c.DrawNumber).NotNull().GreaterThanOrEqualTo(1);
        }
    }
}