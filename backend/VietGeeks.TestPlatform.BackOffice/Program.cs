using Azure.Identity;
using Duende.Bff.EntityFramework;
using Microsoft.EntityFrameworkCore;
using VietGeeks.TestPlatform.AccountManager;
using VietGeeks.TestPlatform.AspNetCore;
using VietGeeks.TestPlatform.SharedKernel;
using VietGeeks.TestPlatform.TestManager.Infrastructure;

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
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.RegisterTestManagerModule(new TestManagerModuleOptions
{
    Database = builder.Configuration.GetSection("TestManagerDatabase").Get<DatabaseOptions>() ?? new DatabaseOptions(),
    ServiceBus = builder.Configuration.GetSection("TestManagerServiceBus").Get<ServiceBusOptions>() ?? new ServiceBusOptions(),
});
builder.Services.RegisterAccountManagerModule(new AccountManagerModuleOptions
{
    Database = builder.Configuration.GetSection("TestManagerDatabase").Get<DatabaseOptions>() ?? new DatabaseOptions(),
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