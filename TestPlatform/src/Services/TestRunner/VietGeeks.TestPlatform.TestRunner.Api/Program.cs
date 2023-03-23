using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using VietGeeks.TestPlaftorm.TestRunner.Infrastructure;
using VietGeeks.TestPlatform.AspNetCore;
using VietGeeks.TestPlatform.TestRunner.Api.Actors;
using VietGeeks.TestPlatform.TestRunner.Api.Swagger;
using VietGeeks.TestPlatform.TestRunner.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddVietGeeksAspNetCore(new()
{
    DataProtection = builder.Configuration.GetSection("DataProtection").Get<DataProtectionOptions>()
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
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

builder.Services.AddDaprClient();
builder.Services.AddActors(options =>
{
    options.Actors.RegisterActor<ProctorActor>();
});

builder.Services.RegisterInfrastructureModule(new()
{
    Database = builder.Configuration.GetSection("TestRunnerDatabase").Get<DatabaseOptions>() ?? new DatabaseOptions()
});

var app = builder.Build();

app.UseCors("dev");

app.MapActorsHandlers();

app.UseVietGeeksEssentialFeatures();

app.UseAuthorization();

app.MapControllers();

app.Run();