using System;
using FluentValidation;
using VietGeeks.TestPlatform.TestManager.Contract;

namespace VietGeeks.TestPlatform.TestManager.Api.ValidationRules;

public class QuestionValidator : AbstractValidator<QuestionViewModel>
{
    public QuestionValidator()
    {

    }
}

public class NewQuestionValidator : AbstractValidator<NewQuestionViewModel>
{
    public NewQuestionValidator()
    {

    }
}
