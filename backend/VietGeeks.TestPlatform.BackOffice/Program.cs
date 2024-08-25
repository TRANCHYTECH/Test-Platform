using Duende.Bff.EntityFramework;
using Microsoft.EntityFrameworkCore;
using VietGeeks.TestPlatform.AccountManager;
using VietGeeks.TestPlatform.AspNetCore;
using VietGeeks.TestPlatform.BackOffice;
using VietGeeks.TestPlatform.SharedKernel;
using VietGeeks.TestPlatform.TestManager.Infrastructure;

const string testPortalSpaPolicy = "test-portal-spa";
const string appName = "test-manager-api";

var builder = WebApplication.CreateBuilder(args);

builder.AddTestPlatformKeyVault(appName);
builder.Services.AddVietGeeksAspNetCore(new VietGeeksAspNetCoreOptions
{
    OpenIdConnect = builder.Configuration.GetSection("Authentication:Schemes:BackOffice").Get<OpenIdConnectOptions>()
});

builder.AddTestManagerMassTransit();

var databaseOptions = new DatabaseOptions
{
    ConnectionString = builder.Configuration.GetConnectionString("TestManager")!,
    DatabaseName = builder.Configuration.GetValue<string>("TestManagerDatabaseName")!,
};

builder.Services.RegisterTestManagerModule(new TestManagerModuleOptions
{
    Database = databaseOptions,
});
builder.Services.RegisterAccountManagerModule(new AccountManagerModuleOptions
{
    Database = databaseOptions,
});

builder.Services.AddControllers();
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
builder.Services.AddAuthorization();

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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors(testPortalSpaPolicy);
// todo: remove this dev block
if (app.Configuration.GetValue<bool>("ApplyMigrationsOnStartup"))
{
    await using var scope = app.Services.CreateAsyncScope();
    await using var context = scope.ServiceProvider.GetRequiredService<SessionDbContext>();
    await context.Database.MigrateAsync(CancellationToken.None);
}

app.MapGet("/test-portal",
        (HttpRequest _) => TypedResults.Redirect(app.Configuration.GetValue<string>("PortalUrl")!, permanent: true))
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