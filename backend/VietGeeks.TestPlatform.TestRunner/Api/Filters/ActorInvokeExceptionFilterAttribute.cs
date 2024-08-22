using Dapr.Actors;
using Microsoft.AspNetCore.Mvc.Filters;
using VietGeeks.TestPlatform.SharedKernel.Exceptions;

namespace VietGeeks.TestPlatform.TestRunner.Api.Filters;

public class ActorInvokeExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is ActorMethodInvocationException ex
            && ex.InnerException is ActorInvokeException invokeEx
            && invokeEx.ActualExceptionType == typeof(TestPlatformException).FullName)
        {
            context.Exception = new TestPlatformException(invokeEx.Message);
        }
    }
}