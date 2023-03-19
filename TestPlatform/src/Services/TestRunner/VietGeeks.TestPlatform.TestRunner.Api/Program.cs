using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using VietGeeks.TestPlaftorm.TestRunner.Infrastructure;
using VietGeeks.TestPlatform.AspNetCore;
using VietGeeks.TestPlatform.TestRunner.Api.Actors;
using VietGeeks.TestPlatform.TestRunner.Api.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.CustomOperationIds(e =>
    {
        var attribute = e.CustomAttributes().FirstOrDefault(x => x.GetType() == typeof(SwaggerOperationAttribute));
        return attribute != null ? ((SwaggerOperationAttribute)attribute).OperationId : e.TryGetMethodInfo(out MethodInfo methodInfo) ? methodInfo.Name : null;
    });
    c.OperationFilter<TestSessionHeaderFilter>();
});
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "dev",
                      builder =>
                      {
                          builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                      });
});

var daprHttpPort = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT") ?? "3600";
var daprGrpcPort = Environment.GetEnvironmentVariable("DAPR_GRPC_PORT") ?? "60000";
builder.Services.AddDaprClient();

builder.Services.AddActors(options =>
{
    options.Actors.RegisterActor<ProctorActor>();
});

var dataOptions = new InfrastructureDataOptions
{
    Database = builder.Configuration.GetSection("TestRunnerDatabase").Get<DatabaseOptions>() ?? new DatabaseOptions()
};

builder.Services.RegisterInfrastructureModule(dataOptions);

var app = builder.Build();

app.UseCors("dev");

app.MapActorsHandlers();

app.UseVietGeeksEssentialFeatures();

app.UseAuthorization();

app.MapControllers();

app.Run();