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

builder.Services.RegisterInfrastructureModule(new()
{
    Database = builder.Configuration.GetSection("TestManagerDatabase").Get<DatabaseOptions>() ?? new DatabaseOptions(),
    ServiceBus = builder.Configuration.GetSection("TestManagerServiceBus").Get<ServiceBusOptions>() ?? new ServiceBusOptions()
});

builder.Services.AddDaprClient();

var app = builder.Build();

app.UseCors("dev");

app.UseVietGeeksEssentialFeatures();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();