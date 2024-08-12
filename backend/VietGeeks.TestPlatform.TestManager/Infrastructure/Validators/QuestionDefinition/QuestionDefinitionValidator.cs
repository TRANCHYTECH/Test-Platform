using FluentValidation;
using VietGeeks.TestPlatform.TestManager.Data.Models;
using VietGeeks.TestPlatform.TestManager.Infrastructure.Services;


namespace VietGeeks.TestPlatform.TestManager.Infrastructure.Validators.QuestionDefinition
{
    public class QuestionDefinitionValidator : AbstractValidator<Data.Models.QuestionDefinition>
    {
        public QuestionDefinitionValidator(
            IQuestionRelatedValidationService validationService,
            IValidator<Answer> validator1,
            IValidator<SingleChoiceScoreSettings> validator2,
            IValidator<MultipleChoiceScoreSettings> validator3)
        {
            //todo: DISCUSSION if there is need for further verification by checking test def status. Example: Finished => Not allow to add question.
            RuleFor(c => c.TestId).NotEmpty().MustAsync((testId, _) => validationService.CheckTestDefinitionExistence(testId)).WithErrorCode("ERR.QUESTIONDEF.TESTSID.NOTFOUND");
            RuleFor(c => c.CategoryId).NotEmpty().MustAsync((categoryId, _) => validationService.CheckTestCategoryExistence(categoryId)).WithErrorCode("ERR.QUESTIONDEF.TESTCATID.NOTFOUND");
            RuleFor(c => c.AnswerType).IsInEnum();
            //todo: define suitable maxlength for question desc.
            RuleFor(c => c.Description).NotEmpty().MaximumLength(3000);
            RuleFor(c => c.Answers).NotEmpty();
            RuleForEach(c => c.Answers).SetValidator(validator1);
            RuleFor(c => c.ScoreSettings).NotNull().Custom(AnswerTypeMatchValidator()).SetInheritanceValidator(v =>
            {
                v.Add(validator2);
                v.Add(validator3);
            });
        }

        private static Action<ScoreSettings, ValidationContext<Data.Models.QuestionDefinition>> AnswerTypeMatchValidator()
        {
            return (prop, ctx) =>
            {
                var checkMatchAnswerType = CheckAnswerTypeMatch(ctx.InstanceToValidate);
                if (checkMatchAnswerType == false)
                {
                    ctx.AddFailureWithErrorCode("ERR.QUESTIONDEF.SCORE.MISMATCH");
                }
            };
        }

        private static bool CheckAnswerTypeMatch(Data.Models.QuestionDefinition setttings)
        {
            switch (setttings.AnswerType)
            {
                case AnswerType.SingleChoice:
                    return setttings.ScoreSettings is SingleChoiceScoreSettings;

                case AnswerType.MultipleChoice:
                    return setttings.ScoreSettings is MultipleChoiceScoreSettings;

                default:
                    return false;
            }
        }
    }

    public class AnswerValidator : AbstractValidator<Answer>
    {
        public AnswerValidator()
        {
            RuleFor(c => c.Id).NotEmpty();
            RuleFor(c => c.AnswerDescription).NotEmpty().MaximumLength(500);
            RuleFor(c => c.AnswerPoint).InclusiveBetween(0, 1000);
        }
    }

    public class SingleChoiceScoreSettingsValidator : AbstractValidator<SingleChoiceScoreSettings>
    {
        public SingleChoiceScoreSettingsValidator()
        {
            RuleFor(c => c.CorrectPoint).InclusiveBetween(0, 1000);
            RuleFor(c => c.IncorrectPoint).InclusiveBetween(-999, 0);
        }
    }

    public class MultipleChoiceScoreSettingsValidator : AbstractValidator<MultipleChoiceScoreSettings>
    {
        public MultipleChoiceScoreSettingsValidator()
        {
            RuleFor(c => c.CorrectPoint).InclusiveBetween(0, 1000);
            RuleFor(c => c.IncorrectPoint).InclusiveBetween(-999, 0);
            When(c => c.IsPartialAnswersEnabled, () =>
            {
                RuleFor(c => c.BonusPoints).InclusiveBetween(0, 1000);
                RuleFor(c => c.PartialIncorrectPoint).InclusiveBetween(-999, 0);
            }).Otherwise(() =>
            {
                RuleFor(c => c.BonusPoints).Must(c => c.GetValueOrDefault() == 0);
                RuleFor(c => c.PartialIncorrectPoint).Must(c => c.GetValueOrDefault() == 0);
            });
        }
    }
}

