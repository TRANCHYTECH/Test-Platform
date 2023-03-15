using System;
using System.Reflection.Metadata;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using VietGeeks.TestPlatform.TestRunner.Api.Controllers;

namespace VietGeeks.TestPlatform.TestRunner.Api.Swagger
{
    public class TestSessionHeaderFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var methods = new string[] { nameof(TestController.ProvideExamineeInfo), nameof(TestController.StartTest), nameof(TestController.SubmitAnswer) };
            if(methods.Contains(context.MethodInfo.Name) == false)
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
}

