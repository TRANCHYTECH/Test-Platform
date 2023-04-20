using FluentValidation.Results;
using VietGeeks.TestPlatform.SharedKernel.Exceptions;

namespace FluentValidation;
//todo: move to shared place
public static class FluentValidationExtensions
{
    public static async Task TryValidate<T>(this IValidator<T> validator, T instance)
    {
        var result = await validator.ValidateAsync(instance);
        CheckResultThrowException(result);
    }

    public static async Task TryValidate<T>(this IValidator<T> validator, T instance, params string[] properties)
    {
        var result = await validator.ValidateAsync(instance, opt => opt.IncludeProperties(properties));
        CheckResultThrowException(result);
    }

    private static void CheckResultThrowException(ValidationResult result)
    {
        if (!result.IsValid)
        {
            //todo: refine error msg details.
            var ex = new ValidationException(result.Errors);
            throw new TestPlatformException(ex.Message, ex);
        }
    }
}