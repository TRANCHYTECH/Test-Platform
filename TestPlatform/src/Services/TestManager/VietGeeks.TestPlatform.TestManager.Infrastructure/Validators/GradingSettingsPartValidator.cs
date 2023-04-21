using System;
using FluentValidation;
using FluentValidation.Results;
using VietGeeks.TestPlatform.SharedKernel.Exceptions;
using VietGeeks.TestPlatform.TestManager.Core.Models;
using VietGeeks.TestPlatform.TestManager.Infrastructure.Services;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure.Validators
{
    public class GradingSettingsPartValidator : AbstractValidator<GradingSettingsPart>
    {
        public GradingSettingsPartValidator(
            IValidator<TestEndConfig> validator1,
            IValidator<PassMaskCriteria> validator2,
            IValidator<GradeRangeCriteria> validator3)
        {
            RuleFor(c => c.TestEndConfig).NotNull().SetValidator(validator1);
            //todo: validate mismatch key and value.
            RuleForEach(c => c.GradingCriterias.Values).SetInheritanceValidator(v =>
            {
                v.Add(validator2);
                v.Add(validator3);
            });
        }
    }

    public class TestEndConfigValidator : AbstractValidator<TestEndConfig>
    {
        public TestEndConfigValidator()
        {
            RuleFor(c => c.Message).NotEmpty().MaximumLength(1000);
            When(c => c.RedirectTo, () =>
            {
                RuleFor(c => c.ToAddress).NotEmpty();
            }).Otherwise(() =>
            {
                RuleFor(c => c.ToAddress).Null();
            });
        }
    }

    public class PassMaskCriteriaValidator : AbstractValidator<PassMaskCriteria>
    {
        public PassMaskCriteriaValidator()
        {
            RuleFor(c => c.Unit).IsInEnum();
            When(c => c.Unit == RangeUnit.Percent, () =>
            {
                RuleFor(c => c.Value).InclusiveBetween(1, 100);
            });
            When(c => c.Unit == RangeUnit.Point, () =>
            {
                RuleFor(c => c.Value).GreaterThan(0);
            });
        }
    }

    public class GradeRangeCriteriaValidator : AbstractValidator<GradeRangeCriteria>
    {
        public GradeRangeCriteriaValidator(IQuestionManagerService questionManagerService)
        {
            RuleFor(c => c.Unit).IsInEnum();

            When(c => c.Unit == RangeUnit.Percent, () =>
            {
                RuleFor(c => c.Details).Custom((details, ctx) =>
                {
                    ctx.RootContextData["MaximumValue"] = 100;
                    ValidateGradeRangeCriteriaDetails(details, ctx);
                });
            });

            When(c => c.Unit == RangeUnit.Point, () =>
            {
                RuleFor(c => c.Details).Custom(async (details, ctx) =>
                {
                    //todo: get max value from question.
                    var testId  = (string)ctx.RootContextData["TestId"];
                    var totalPoints = await questionManagerService.GetTotalPoints(testId, default);
                    ctx.RootContextData["MaximumValue"] = totalPoints;
                    ValidateGradeRangeCriteriaDetails(details, ctx);
                });
            });

            static void ValidateGradeRangeCriteriaDetails(List<GradeRangeCriteriaDetail> details, ValidationContext<GradeRangeCriteria> ctx)
            {
                int maxValue = (int)ctx.RootContextData["MaximumValue"];
                var criteria = ctx.InstanceToValidate;
                var orderedRanges = details.OrderBy(c => c.To);

                // Check overlapsed range.
                var overllapsedRange = orderedRanges.GroupBy(c => c.To).FirstOrDefault(c => c.Count() > 1);
                if (overllapsedRange != null)
                {
                    ctx.AddFailure(new ValidationFailure(ctx.PropertyName, "ERR.TESTDEF.GRADE.001"));
                    return;
                }

                // First To must greater than 0.
                if (orderedRanges.First().To == 0)
                {

                    ctx.AddFailure(new ValidationFailure(ctx.PropertyName, "ERR.TESTDEF.GRADE.002"));
                    return;
                }

                // Last much 100
                if (orderedRanges.Last().To != maxValue)
                {
                    ctx.AddFailure(new ValidationFailure(ctx.PropertyName, "ERR.TESTDEF.GRADE.003"));
                    return;
                }

                // All grade fields.
                var gradeFields = GetGradeFields(criteria.GradeType).ToList();
                var allGradeFieldsHasData = details.TrueForAll(c =>
                {
                    return gradeFields.TrueForAll(grade => c.Grades.ContainsKey(grade) && !string.IsNullOrWhiteSpace(c.Grades[grade]));
                });

                if (!allGradeFieldsHasData)
                {
                    ctx.AddFailure(new ValidationFailure(ctx.PropertyName, "ERR.TESTDEF.GRADE.004"));
                }
            }
        }

        protected override bool PreValidate(ValidationContext<GradeRangeCriteria> context, ValidationResult result)
        {
            if(!context.RootContextData.ContainsKey("TestId"))
            {
                result.Errors.Add(new ValidationFailure("RootContextData['TestId']", "TestId is required"));

                return false;
            }

            return true;
        }

        public static string[] GetGradeFields(GradeType gradeType)
        {
            switch (gradeType)
            {
                case GradeType.Grade:
                    return new[] { $"{(int)GradeType.Grade}" };
                case GradeType.Descriptive:
                    return new[] { $"{(int)GradeType.Descriptive}" };
                case GradeType.GradeAndDescriptive:
                    return new[] { $"{(int)GradeType.Grade}", $"{(int)GradeType.Descriptive}" };
                default:
                    throw new TestPlatformException("Not supported type");
            }
        }
    }
}

