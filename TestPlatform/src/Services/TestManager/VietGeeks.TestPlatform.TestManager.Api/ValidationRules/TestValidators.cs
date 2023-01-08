using System;
using FluentValidation;
using VietGeeks.TestPlatform.TestManager.Contract;

namespace VietGeeks.TestPlatform.TestManager.Api.ValidationRules
{
    public class NewTestValidator: AbstractValidator<NewTestDefinitionViewModel>
    {
        public NewTestValidator()
        {
            //RuleFor(c => c.Name).NotEmpty();
            //RuleFor(c => c.Category).NotEmpty();
        }
    }
}

