using System;
using FluentValidation;
using VietGeeks.TestPlatform.TestManager.Contract;

namespace VietGeeks.TestPlatform.TestManager.Api.ValidationRules
{
    public class NewTestValidator: AbstractValidator<NewTestViewModel>
    {
        public NewTestValidator()
        {
            RuleFor(c => c.TestName).NotEmpty();
            RuleFor(c => c.CategoryId).NotEmpty();
            RuleFor(c => c.LanguageId).NotEmpty();
        }
    }
}

