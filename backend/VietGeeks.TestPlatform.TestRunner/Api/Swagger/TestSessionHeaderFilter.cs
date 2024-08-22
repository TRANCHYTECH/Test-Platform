using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using VietGeeks.TestPlatform.TestRunner.Api.Controllers;

namespace VietGeeks.TestPlatform.TestRunner.Api.Swagger;

public class TestSessionHeaderFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var methods = new[]
        {
            nameof(ExamController.ProvideExamineeInfo),
            nameof(ExamController.StartExam),
            nameof(ExamController.SubmitAnswer),
            nameof(ExamController.FinishExam)
        };
        if (methods.Contains(context.MethodInfo.Name) == false)
        {
            return;
        }

        if (operation.Parameters == null)
        {
            operation.Parameters = new List<OpenApiParameter>();
        }

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "TestSession",
            In = ParameterLocation.Header,
            Required = true,
            Schema = new OpenApiSchema
            {
                Type = "string"
            }
        });
    }
}