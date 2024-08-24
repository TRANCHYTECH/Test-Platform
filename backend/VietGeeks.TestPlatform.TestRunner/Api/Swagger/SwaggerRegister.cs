using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace VietGeeks.TestPlatform.TestRunner.Api.Swagger
{
    public static class SwaggerRegister
    {
        public static void AddTestRunnerSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.CustomOperationIds(e =>
                {
                    var attribute = e.CustomAttributes().FirstOrDefault(x => x.GetType() == typeof(SwaggerOperationAttribute));
                    if (attribute != null)
                    {
                        return ((SwaggerOperationAttribute)attribute).OperationId;
                    }

                    return e.TryGetMethodInfo(out var methodInfo) ? methodInfo.Name : null;
                });
                c.OperationFilter<TestSessionHeaderFilter>();
            });
        }
    }
}