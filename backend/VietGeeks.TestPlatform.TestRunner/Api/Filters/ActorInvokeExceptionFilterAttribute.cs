using Dapr.Actors;
using VietGeeks.TestPlatform.SharedKernel.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace VietGeeks.TestPlatform.TestRunner.Api.Filters;

public class ActorInvokeExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is ActorMethodInvocationException ex
        && (ex.InnerException is Dapr.Actors.ActorInvokeException invokeEx)
        && invokeEx.ActualExceptionType == typeof(TestPlatformException).FullName)
        {
            context.Exception = new TestPlatformException(invokeEx.Message);
        }
    }
}