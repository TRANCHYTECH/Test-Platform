using VietGeeks.TestPlatform.AspNetCore;
using VietGeeks.TestPlatform.TestManager.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddVietGeeksAspNetCore(new()
{
    Auth = builder.Configuration.GetSection("Auth").Get<AuthOptions>()
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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


builder.Services.RegisterTestManagerModule(new()
{
    Database = builder.Configuration.GetSection("TestManagerDatabase").Get<DatabaseOptions>() ?? new DatabaseOptions(),
    ServiceBus = builder.Configuration.GetSection("TestManagerServiceBus").Get<ServiceBusOptions>() ?? new ServiceBusOptions()
});
builder.Services.RegisterTestManagerModule(new()
{
    Database = builder.Configuration.GetSection("AccountSettingsDatabase").Get<DatabaseOptions>() ?? new DatabaseOptions()
});

builder.Services.AddDaprClient();

builder.Services
    .AddAuthorization();

var app = builder.Build();

app.UseCors("dev");

app.UseVietGeeksEssentialFeatures();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapSubscribeHandler();

app.Run();