using Azure.Identity;
using Duende.Bff.EntityFramework;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Entities;
using VietGeeks.TestPlatform.AccountManager;
using VietGeeks.TestPlatform.AspNetCore;
using VietGeeks.TestPlatform.SharedKernel;
using VietGeeks.TestPlatform.TestManager.Infrastructure;
using VietGeeks.TestPlatform.TestManager.Infrastructure.EventConsumers;

const string testPortalSpaPolicy = "test-portal-spa";

var builder = WebApplication.CreateBuilder(args);
if (builder.Environment.IsProduction())
{
    builder.Configuration.AddAzureKeyVault(
        new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"),
        new DefaultAzureCredential());
}

builder.Services.AddControllers();
builder.Services.AddVietGeeksAspNetCore(new VietGeeksAspNetCoreOptions
{
    OpenIdConnect = builder.Configuration.GetSection("Authentication:Schemes:BackOffice").Get<OpenIdConnectOptions>(),
});
builder.Services.AddMassTransit(c =>
{
    c.ConfigureHealthCheckOptions(cfg => cfg.MinimalFailureStatus = HealthStatus.Degraded);
    c.AddMongoDbOutbox(o =>
    {
        var dbOptions = GetTestManagerDatabaseOptions(builder);
        o.ClientFactory(_ => DB.Database(dbOptions.DatabaseName).Client);
        o.DatabaseFactory(_ => DB.Database(dbOptions.DatabaseName));
        o.DuplicateDetectionWindow = TimeSpan.FromSeconds(30);
        o.UseBusOutbox();
    });

    var endpointPrefix = builder.Environment.IsDevelopment()
        ? Environment.UserName
        : string.Empty;
    c.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter(endpointPrefix, includeNamespace: false));
    c.AddConsumersFromNamespaceContaining<SendTestAccessCode>();
    c.UsingAzureServiceBus((ctx, factoryConfig) =>
    {
        factoryConfig.Host(builder.Configuration.GetConnectionString("ServiceBus"),
            hostConfig =>
            {
                if (builder.Environment.IsProduction())
                {
                    hostConfig.TokenCredential = new DefaultAzureCredential();
                }
            });
        factoryConfig.ConfigureEndpoints(ctx);
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.RegisterTestManagerModule(new TestManagerModuleOptions
{
    Database = GetTestManagerDatabaseOptions(builder),
});
builder.Services.RegisterAccountManagerModule(new AccountManagerModuleOptions
{
    Database = GetTestManagerDatabaseOptions(builder),
});
builder.Services.AddCors(options =>
{
    options.AddPolicy(testPortalSpaPolicy,
        policyBuilder =>
        {
            policyBuilder
                .WithOrigins(builder.Configuration.GetValue<string>("PortalUrl")!)
                .AllowCredentials()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});
builder.Services.AddDaprClient();

builder.Services
    .AddAuthorization();

builder.Services.Configure<SessionStoreOptions>(options => options.DefaultSchema = "session");
builder.Services.AddBff(options =>
{
    options.BackchannelLogoutAllUserSessions = true;
    options.EnableSessionCleanup = true;
}).AddEntityFrameworkServerSideSessions<SessionDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("UserSession"), sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(3);
        sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "session");
        sqlOptions.MigrationsAssembly(typeof(Program).Assembly.FullName);
    });
});

var app = builder.Build();
app.UseCors(testPortalSpaPolicy);

if (app.Configuration.GetValue<bool>("ApplyMigrationsOnStartup"))
{
    await using var scope = app.Services.CreateAsyncScope();
    await using var context = scope.ServiceProvider.GetRequiredService<SessionDbContext>();
    await context.Database.MigrateAsync(CancellationToken.None);
}

app.MapGet("/test-portal", (HttpRequest _) => TypedResults.Redirect(app.Configuration.GetValue<string>("PortalUrl")!, permanent: true))
    .WithSummary("Redirect after user login via portal")
    .WithOpenApi();

app.UseVietGeeksEssentialFeatures();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseBff();
app.UseAuthorization();
app.MapBffManagementEndpoints();
app.MapControllers().RequireAuthorization().AsBffApiEndpoint().SkipAntiforgery();
app.MapSubscribeHandler();

await app.RunAsync();
return;

DatabaseOptions GetTestManagerDatabaseOptions(WebApplicationBuilder webApplicationBuilder)
{
    return webApplicationBuilder.Configuration.GetSection("TestManagerDatabase").Get<DatabaseOptions>()!;
}