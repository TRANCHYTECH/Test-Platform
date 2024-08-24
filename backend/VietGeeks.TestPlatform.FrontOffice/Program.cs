using VietGeeks.TestPlatform.AspNetCore;
using VietGeeks.TestPlatform.SharedKernel.PureServices;
using VietGeeks.TestPlatform.TestRunner.Api.Actors;
using VietGeeks.TestPlatform.TestRunner.Api.Filters;
using VietGeeks.TestPlatform.TestRunner.Api.Swagger;
using VietGeeks.TestPlatform.TestRunner.Infrastructure;

const string testRunnerSpaPolicy = "test-runner-spa";
const string appName = "test-runner-api";

var builder = WebApplication.CreateBuilder(args);

builder.AddTestPlatformKeyVault(appName);
builder.AddTestPlatformDataProtection(appName);
builder.Services.RegisterTestRunnerModule(builder.Configuration);
builder.Services.AddSingleton<IClock, Clock>();
builder.Services.AddControllers(c => c.Filters.Add<ActorInvokeExceptionFilterAttribute>());
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
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

builder.Services.AddDaprClient();
builder.Services.AddActors(options => options.Actors.RegisterActor<ProctorActor>());

//todo: use distributed cache
builder.Services.AddMemoryCache();

builder.Services.AddTestRunnerSwagger();

var app = builder.Build();
app.UseCors(testRunnerSpaPolicy);
app.UseVietGeeksEssentialFeatures();
app.MapActorsHandlers();
app.MapControllers();

await app.RunAsync();