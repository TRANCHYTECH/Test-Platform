using Duende.Bff.EntityFramework;
using Microsoft.EntityFrameworkCore;
using VietGeeks.TestPlatform.AccountManager;
using VietGeeks.TestPlatform.AspNetCore;
using VietGeeks.TestPlatform.SharedKernel;
using VietGeeks.TestPlatform.TestManager.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddVietGeeksAspNetCore(new VietGeeksAspNetCoreOptions
{
    OpenIdConnect = builder.Configuration.GetSection("Authentication:Schemes:BackOffice").Get<OpenIdConnectOptions>(),
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "dev",
                      configure =>
                      {
                          configure
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                      });
});

builder.Services.RegisterTestManagerModule(new TestManagerModuleOptions
{
    Database = builder.Configuration.GetSection("TestManagerDatabase").Get<DatabaseOptions>() ?? new DatabaseOptions(),
    ServiceBus = builder.Configuration.GetSection("TestManagerServiceBus").Get<ServiceBusOptions>() ?? new ServiceBusOptions(),
});
builder.Services.RegisterAccountManagerModule(new AccountManagerModuleOptions
{
    Database = builder.Configuration.GetSection("AccountManagerDatabase").Get<DatabaseOptions>() ?? new DatabaseOptions(),
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
if (app.Configuration.GetValue<bool>("ApplyMigrationsOnStartup"))
{
    await using var scope = app.Services.CreateAsyncScope();
    await using var context = scope.ServiceProvider.GetRequiredService<SessionDbContext>();
    await context.Database.MigrateAsync(CancellationToken.None);
}

//todo: only dev
app.UseCors("dev");

app.UseVietGeeksEssentialFeatures();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapSubscribeHandler();

await app.RunAsync();