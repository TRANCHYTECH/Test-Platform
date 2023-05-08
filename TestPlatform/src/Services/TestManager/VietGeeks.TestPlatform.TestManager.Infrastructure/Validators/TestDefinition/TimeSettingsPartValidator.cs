using FluentValidation;
using FluentValidation.Results;
using VietGeeks.TestPlatform.SharedKernel.PureServices;
using VietGeeks.TestPlatform.TestManager.Core.Models;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure.Validators.TestDefintion;

public class TimeSettingsPartValidator : AbstractValidator<TimeSettingsPart>
{
    public TimeSettingsPartValidator(
        IValidator<CompleteQuestionDuration> validator1,
        IValidator<CompleteTestDuration> validator2,
        IValidator<ManualTestActivation> validator3,
        IValidator<TimePeriodActivation> validator4,
        IValidator<AnswerQuestionConfig> validator5)
    {
        RuleFor(c => c.TestDurationMethod).NotNull().SetInheritanceValidator(v =>
        {
            v.Add<CompleteQuestionDuration>(validator1);
            v.Add<CompleteTestDuration>(validator2);
        });

        RuleFor(c => c.TestActivationMethod).NotNull().SetInheritanceValidator(v =>
        {
            v.Add<ManualTestActivation>(validator3);
            v.Add<TimePeriodActivation>(validator4);
        });

        RuleFor(c => c.AnswerQuestionConfig).NotNull().SetValidator(validator5);
    }

    protected override bool PreValidate(ValidationContext<TimeSettingsPart> context, ValidationResult result)
    {
        context.RootContextData["TimeSettingsPart.TestDurationMethod"] = context.InstanceToValidate.TestDurationMethod;

        return true;
    }
}

public class CompleteQuestionDurationValidator : AbstractValidator<CompleteQuestionDuration>
{
    public CompleteQuestionDurationValidator()
    {
        // Range [00:00:01, 00:59:59]
        RuleFor(c => c.Duration).ExclusiveBetween(TimeSpan.FromSeconds(0), TimeSpan.FromHours(1));
    }
}

public class CompleteTestDurationValidator : AbstractValidator<CompleteTestDuration>
{
    public CompleteTestDurationValidator()
    {
        // Range [00:00:01, 23:59:59]
        RuleFor(c => c.Duration).ExclusiveBetween(TimeSpan.FromSeconds(0), TimeSpan.FromHours(24));
    }
}

public class ManualTestActivationValidator : AbstractValidator<ManualTestActivation>
{
    public ManualTestActivationValidator()
    {
        // Range [00:00:01, 99.23:59:59]
        RuleFor(c => c.ActiveUntil).ExclusiveBetween(TimeSpan.FromSeconds(0), TimeSpan.FromDays(100));
    }
}

public class TimePeriodActivationValidator : AbstractValidator<TimePeriodActivation>
{
    public TimePeriodActivationValidator(IClock clock)
    {
        // Rule: UtcNow < ActiveFromDate < ActiveUntilDate
        RuleFor(c => c.ActiveFromDate).GreaterThan(clock.UtcNow);
        RuleFor(c => c.ActiveUntilDate).GreaterThan(c => c.ActiveFromDate);
    }
}

public class AnswerQuestionConfigValidator : AbstractValidator<AnswerQuestionConfig>
{
    public AnswerQuestionConfigValidator()
    {
        RuleFor(c => c.SkipQuestion).Must((prop, field, ctx) =>
        {
            var testDurationMethod = ctx.RootContextData["TimeSettingsPart.TestDurationMethod"] as TestDurationMethod;
            if(testDurationMethod is CompleteQuestionDuration && field == true) {
                return false;
            }

            return true;
        }).WithErrorCode("ERR.TESTDEF.TIME.001"); // When test duration method is CompleteQuestionDuration, not allow to skip question.
    }
}

