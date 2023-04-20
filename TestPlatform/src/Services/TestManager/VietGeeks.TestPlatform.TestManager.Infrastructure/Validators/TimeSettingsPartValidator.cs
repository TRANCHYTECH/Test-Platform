using FluentValidation;
using VietGeeks.TestPlatform.TestManager.Core.Models;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure.Validators;

public class TimeSettingsPartValidator : AbstractValidator<TimeSettingsPart>
{
    public TimeSettingsPartValidator(
        IValidator<CompleteQuestionDuration> completeQuestionDurationValidator,
        IValidator<CompleteTestDuration> completeTestDurationValidator,
        IValidator<ManualTestActivation> manualTestActivationValidator,
        IValidator<TimePeriodActivation> timePeriodActivationValidator,
        IValidator<AnswerQuestionConfig> answerQuestionConfigValidator)
    {
        RuleFor(c => c.TestDurationMethod).SetInheritanceValidator(v =>
        {
            v.Add<CompleteQuestionDuration>(completeQuestionDurationValidator);
            v.Add<CompleteTestDuration>(completeTestDurationValidator);
        });

        RuleFor(c => c.TestActivationMethod).SetInheritanceValidator(v =>
        {
            v.Add<ManualTestActivation>(manualTestActivationValidator);
            v.Add<TimePeriodActivation>(timePeriodActivationValidator);
        });

        RuleFor(c => c.AnswerQuestionConfig).SetValidator(answerQuestionConfigValidator);
    }
}

public class CompleteQuestionDurationValidator : AbstractValidator<CompleteQuestionDuration>
{
    public CompleteQuestionDurationValidator()
    {
        RuleFor(c => c.Duration).GreaterThan(TimeSpan.Zero);
    }
}

public class CompleteTestDurationValidator : AbstractValidator<CompleteTestDuration>
{
    public CompleteTestDurationValidator()
    {
        RuleFor(c => c.Duration).GreaterThan(TimeSpan.Zero);
    }
}

public class ManualTestActivationValidator : AbstractValidator<ManualTestActivation>
{
    public ManualTestActivationValidator()
    {
        RuleFor(c => c.ActiveUntil)
        .NotNull()
        .LessThanOrEqualTo(TimeSpan.FromDays(99));
    }
}

public class TimePeriodActivationValidator : AbstractValidator<TimePeriodActivation>
{
    public TimePeriodActivationValidator()
    {
        RuleFor(c => c.ActiveFromDate).GreaterThanOrEqualTo(DateTime.UtcNow);
        RuleFor(c => c.ActiveUntilDate).GreaterThan(c => c.ActiveFromDate);
    }
}

public class AnswerQuestionConfigValidator : AbstractValidator<AnswerQuestionConfig>
{
    public AnswerQuestionConfigValidator()
    {
        RuleFor(c => c.SkipQuestion).NotNull();
    }
}

