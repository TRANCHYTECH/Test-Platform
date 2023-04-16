using VietGeeks.TestPlatform.AspNetCore;
using VietGeeks.TestPlatform.AccountManager.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddDapr();
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
    Database = builder.Configuration.GetSection("AccountSettingsDatabase").Get<DatabaseOptions>() ?? new DatabaseOptions()
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors("dev");

app.UseVietGeeksEssentialFeatures();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCloudEvents();
app.UseAuthorization();
app.MapControllers();
app.MapSubscribeHandler();

app.Run();

