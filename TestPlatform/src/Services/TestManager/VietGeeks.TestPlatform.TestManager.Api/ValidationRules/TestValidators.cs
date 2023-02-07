using System;
using FluentValidation;
using FluentValidation.Results;
using VietGeeks.TestPlatform.TestManager.Contract;

namespace VietGeeks.TestPlatform.TestManager.Api.ValidationRules;

public class NewTestValidator : AbstractValidator<NewTestDefinitionViewModel>
{
    public NewTestValidator()
    {
        //RuleFor(c => c.Name).NotEmpty();
        //RuleFor(c => c.Category).NotEmpty();
    }
}

public class TestBasicSettingsValidator : AbstractValidator<CreateOrUpdateTestBasicSettingsViewModel>
{
    public TestBasicSettingsValidator()
    {
        RuleFor(c => c.Name).NotEmpty();
    }
}

public class TestSetSettingsValidator: AbstractValidator<CreateOrUpdateTestSetSettingsViewModel>
{
    public TestSetSettingsValidator()
    {
        RuleFor(c => c.GeneratorType).IsInEnum();
    }
}

public class CreateTestDefinitionValidator : AbstractValidator<NewTestDefinitionViewModel>
{
    public CreateTestDefinitionValidator()
    {
        RuleFor(c => c.BasicSettings).NotNull().SetValidator((c) => new TestBasicSettingsValidator());
    }
}

public class UpdateTestDefinitionValidator : AbstractValidator<UpdateTestDefinitionViewModel>
{
    //public UpdateTestDefinitionValidator()
    //{
    //    RuleFor(c => c.BasicSettings).SetValidator("").When(c => c.BasicSettings != null);
    //    RuleFor(c => c.TestSetSettings).SetValidator((c) => new TestSetSettingsValidator()).When(c => c.TestSetSettings != null);
    //}
}

