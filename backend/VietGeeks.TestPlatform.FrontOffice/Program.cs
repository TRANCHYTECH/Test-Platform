using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using Azure.Identity;
using VietGeeks.TestPlatform.AspNetCore;
using VietGeeks.TestPlatform.TestRunner.Api.Actors;
using VietGeeks.TestPlatform.TestRunner.Api.Filters;
using VietGeeks.TestPlatform.TestRunner.Api.Swagger;
using VietGeeks.TestPlatform.TestRunner.Infrastructure;

const string testRunnerSpaPolicy = "test-runner-spa";

var builder = WebApplication.CreateBuilder(args);
if (builder.Environment.IsProduction())
{
    builder.Configuration.AddAzureKeyVault(
        new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"),
        new DefaultAzureCredential());
}
builder.Services.AddVietGeeksAspNetCore(new()
{
    DataProtection = builder.Configuration.GetSection("DataProtection").Get<DataProtectionOptions>()
});
builder.Services.RegisterTestRunnerModule(new()
{
    Database = builder.Configuration.GetSection("TestManagerDatabase").Get<DatabaseOptions>()!
});
builder.Services.AddControllers(c => c.Filters.Add<ActorInvokeExceptionFilterAttribute>());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.CustomOperationIds(e =>
    {
        var attribute = e.CustomAttributes().FirstOrDefault(x => x.GetType() == typeof(SwaggerOperationAttribute));
        if (attribute != null)
        {
            return ((SwaggerOperationAttribute)attribute).OperationId;
        }

        if (e.TryGetMethodInfo(out MethodInfo methodInfo))
        {
            return methodInfo.Name;
        }

        return null;
    });
    c.OperationFilter<TestSessionHeaderFilter>();
});
builder.Services.AddCors(options =>
{
    options.AddPolicy(testRunnerSpaPolicy,
                      policyBuilder =>
                      {
                          policyBuilder
                              .WithOrigins(builder.Configuration.GetValue<string>("TestRunnerUrl")!)
                              .AllowCredentials()
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                      });
});

builder.Services.AddDaprClient();
builder.Services.AddActors(options =>
{
    options.Actors.RegisterActor<ProctorActor>();
});

builder.Services.ConfigureApplicationCookie((options) =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

builder.Services.AddMemoryCache();

var app = builder.Build();
app.UseCors(testRunnerSpaPolicy);
app.MapActorsHandlers();
app.UseVietGeeksEssentialFeatures();
app.MapControllers();

await app.RunAsync();