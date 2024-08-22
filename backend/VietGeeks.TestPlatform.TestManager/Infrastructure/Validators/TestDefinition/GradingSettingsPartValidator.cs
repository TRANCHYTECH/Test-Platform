using FluentValidation;
using FluentValidation.Results;
using VietGeeks.TestPlatform.SharedKernel;
using VietGeeks.TestPlatform.SharedKernel.Exceptions;
using VietGeeks.TestPlatform.TestManager.Data.Models;
using VietGeeks.TestPlatform.TestManager.Infrastructure.Services;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure.Validators.TestDefinition
{
    public class GradingSettingsPartValidator : AbstractValidator<GradingSettingsPart>
    {
        public GradingSettingsPartValidator(
            IValidator<TestEndConfig> validator1,
            IValidator<PassMaskCriteria> validator2,
            IValidator<GradeRangeCriteria> validator3,
            IValidator<InformRespondentConfig> validator4)
        {
            RuleFor(c => c.TestEndConfig).NotNull().SetValidator(validator1);
            //todo: validate mismatch key and value.
            RuleForEach(c => c.GradingCriterias.Values).SetInheritanceValidator(v =>
            {
                v.Add(validator2);
                v.Add(validator3);
            });
            RuleFor(c => c.InformRespondentConfig).NotNull().SetValidator(validator4);
        }

        protected override bool PreValidate(ValidationContext<GradingSettingsPart> context, ValidationResult result)
        {
            context.RootContextData["GradingSettingsPart.GradingCriterias"] =
                context.InstanceToValidate.GradingCriterias;

            return true;
        }
    }

    public class TestEndConfigValidator : AbstractValidator<TestEndConfig>
    {
        public TestEndConfigValidator()
        {
            RuleFor(c => c.Message).NotEmpty().MaximumLength(1000);
            When(c => c.RedirectTo, () => { RuleFor(c => c.ToAddress).NotEmpty(); })
                .Otherwise(() => { RuleFor(c => c.ToAddress).Null(); });
        }
    }

    public class PassMaskCriteriaValidator : AbstractValidator<PassMaskCriteria>
    {
        public PassMaskCriteriaValidator()
        {
            RuleFor(c => c.Unit).IsInEnum();
            When(c => c.Unit == RangeUnit.Percent, () => { RuleFor(c => c.Value).InclusiveBetween(1, 100); });
            When(c => c.Unit == RangeUnit.Point, () => { RuleFor(c => c.Value).GreaterThan(0); });
        }
    }

    public class GradeRangeCriteriaValidator : AbstractValidator<GradeRangeCriteria>
    {
        private const string MaximumValue = "GradeRangeCriteria.MaximumValue";

        public GradeRangeCriteriaValidator(IQuestionManagerService questionManagerService)
        {
            RuleFor(c => c.Unit).IsInEnum();

            When(c => c.Unit == RangeUnit.Percent, () =>
            {
                RuleFor(c => c.Details).Custom((details, ctx) =>
                {
                    ctx.RootContextData[MaximumValue] = 100;
                    ValidateGradeRangeCriteriaDetails(details, ctx);
                });
            });

            When(c => c.Unit == RangeUnit.Point, () =>
            {
                RuleFor(c => c.Details).Custom(async (details, ctx) =>
                {
                    var testId = (string)ctx.RootContextData["TestId"];
                    var totalPoints = await questionManagerService.GetTotalPoints(testId, default);
                    ctx.RootContextData[MaximumValue] = totalPoints;
                    ValidateGradeRangeCriteriaDetails(details, ctx);
                });
            });
            return;

            static void ValidateGradeRangeCriteriaDetails(IList<GradeRangeCriteriaDetail> details,
                ValidationContext<GradeRangeCriteria> ctx)
            {
                var maxValue = (int)ctx.RootContextData[MaximumValue];
                var criteria = ctx.InstanceToValidate;
                var orderedRanges = details.OrderBy(c => c.To).ToArray();

                // Check overlapped range.
                var overlappedRange = orderedRanges.GroupBy(c => c.To).FirstOrDefault(c => c.Count() > 1);
                if (overlappedRange != null)
                {
                    ctx.AddFailure(new ValidationFailure(ctx.PropertyPath, "ERR.TESTDEF.GRADE.001"));
                    return;
                }

                // First To must greater than 0.
                if (orderedRanges[0].To == 0)
                {
                    ctx.AddFailure(new ValidationFailure(ctx.PropertyPath, "ERR.TESTDEF.GRADE.002"));
                    return;
                }

                // Last much 100
                if (orderedRanges[^1].To != maxValue)
                {
                    ctx.AddFailure(new ValidationFailure(ctx.PropertyPath, "ERR.TESTDEF.GRADE.003"));
                    return;
                }

                // All grade fields.
                var gradeFields = GetGradeFields(criteria.GradeType).ToList();
                var allGradeFieldsHasData = details.All(c =>
                {
                    return gradeFields.TrueForAll(grade =>
                        c.Grades.ContainsKey(grade) && !string.IsNullOrWhiteSpace(c.Grades[grade]));
                });

                if (!allGradeFieldsHasData)
                {
                    ctx.AddFailure(new ValidationFailure(ctx.PropertyPath, "ERR.TESTDEF.GRADE.004"));
                }
            }
        }

        protected override bool PreValidate(ValidationContext<GradeRangeCriteria> context, ValidationResult result)
        {
            if (context.RootContextData.ContainsKey("TestId"))
            {
                return true;
            }

            result.Errors.Add(new ValidationFailure("RootContextData['TestId']", "TestId is required"));

            return false;
        }

        private static string[] GetGradeFields(GradeType gradeType)
        {
            return gradeType switch
            {
                GradeType.Grade => [GradeType.Grade.Value()],
                GradeType.Descriptive => [GradeType.Descriptive.Value()],
                GradeType.GradeAndDescriptive => [GradeType.Grade.Value(), GradeType.Descriptive.Value()],
                _ => throw new TestPlatformException("Not supported type")
            };
        }
    }

    public class InformRespondentConfigValidator : AbstractValidator<InformRespondentConfig>
    {
        public InformRespondentConfigValidator()
        {
            // All in enum
            RuleForEach(c => c.InformFactors).Must(c => Enum.TryParse<InformFactor>(c.Key, out var _))
                .WithMessage("ERR.TESTDEF.GRADE.301");

            RuleFor(c => c.InformFactors).Custom((prop, ctx) =>
            {
                // Passed/failed message
                var gradingCriterias =
                    (Dictionary<string, GradingCriteriaConfig>)ctx.RootContextData[
                        "GradingSettingsPart.GradingCriterias"];

                if (prop.TryGetValue(InformFactor.PassOrFailMessage.Value(), out var passOrFailMessage) &&
                    passOrFailMessage)
                {
                    if (!gradingCriterias.ContainsKey(GradingCriteriaConfigType.PassMask.Value()))
                    {
                        ctx.AddFailure("ERR.TESTDEF.GRADE.302");
                    }

                    //todo: Check required 2 messages.
                }

                // Grade & Descriptive grade
                ValidateGradeFactor(prop, InformFactor.Grade, ctx, gradingCriterias, [
                    GradeType.Grade, GradeType.GradeAndDescriptive
                ]);
                ValidateGradeFactor(prop, InformFactor.DescriptiveGrade, ctx, gradingCriterias, [
                    GradeType.Descriptive, GradeType.GradeAndDescriptive
                ]);
            });

            static void ValidateGradeFactor(IDictionary<string, bool> prop, InformFactor factorName,
                ValidationContext<InformRespondentConfig> ctx,
                Dictionary<string, GradingCriteriaConfig> gradingCriterias, GradeType[] gradeTypes)
            {
                if (prop.TryGetValue(factorName.Value(), out var factorValue) && factorValue)
                {
                    // No grade range criteria.
                    if (!gradingCriterias.ContainsKey(GradingCriteriaConfigType.GradeRanges.Value()))
                    {
                        ctx.AddFailure("ERR.TESTDEF.GRADE.303");
                    }
                    else
                    {
                        // No acceptable grade type of the grade range criteria.
                        var gradeType =
                            ((GradeRangeCriteria)gradingCriterias[GradingCriteriaConfigType.GradeRanges.Value()])
                            .GradeType;
                        if (gradeTypes.Any(c => c == gradeType) == false)
                        {
                            ctx.AddFailure("ERR.TESTDEF.GRADE.304");
                        }
                    }
                }
            }
        }
    }
}