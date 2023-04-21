using System;
using FluentValidation;
using VietGeeks.TestPlatform.TestManager.Core.Models;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure.Validators;

public class TestAccessSettingsPartValidator : AbstractValidator<TestAccessSettingsPart>
{
    public TestAccessSettingsPartValidator(
        IValidator<PublicLinkType> validator1,
        IValidator<PrivateAccessCodeType> validator2,
        IValidator<GroupPasswordType> validator3,
        IValidator<TrainingType> validator4)
    {
        RuleFor(c => c.Settings).NotNull().Must(MatchTestAccessType()).WithMessage("Mismatch Test Access Type").SetInheritanceValidator(v =>
        {
            v.Add(validator1);
            v.Add(validator2);
            v.Add(validator3);
            v.Add(validator4);
        });
    }

    private static Func<TestAccessSettingsPart, TestAccessSettings, bool> MatchTestAccessType()
    {
        return (part, prop) =>
        {
            switch (part.AccessType)
            {
                case TestAcessType.PublicLink:
                    return prop is PublicLinkType;
                case TestAcessType.PrivateAccessCode:
                    return prop is PrivateAccessCodeType;
                case TestAcessType.GroupPassword:
                    return prop is GroupPasswordType;
                case TestAcessType.Training:
                    return prop is TrainingType;
                default:
                    return false;
            }
        };
    }
}

public class PublicLinkTypeValidator : AbstractValidator<PublicLinkType>
{
    public PublicLinkTypeValidator()
    {
        RuleFor(c => c.RequireAccessCode).NotNull();
        RuleFor(c => c.Attempts).GreaterThanOrEqualTo(1);
    }
}

public class PrivateAccessCodeTypeValidator : AbstractValidator<PrivateAccessCodeType>
{
    public PrivateAccessCodeTypeValidator(IValidator<PrivateAccessCodeConfig> validator1)
    {
        RuleForEach(c => c.Configs).NotNull().SetValidator(validator1);
        RuleFor(c => c.Attempts).GreaterThanOrEqualTo(1);
    }
}

public class PrivateAccessCodeConfigValidator : AbstractValidator<PrivateAccessCodeConfig>
{
    public PrivateAccessCodeConfigValidator()
    {
        RuleFor(c => c.Code).NotEmpty();
        RuleFor(c => c.Email).EmailAddress().When(c => c.Email != null);
    }
}

public class GroupPasswordTypeValidator : AbstractValidator<GroupPasswordType>
{

}

public class TrainingTypeValidator : AbstractValidator<TrainingType>
{

}
