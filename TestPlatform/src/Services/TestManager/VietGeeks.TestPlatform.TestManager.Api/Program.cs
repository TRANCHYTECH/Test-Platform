using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using VietGeeks.TestPlatform.AspNetCore;
using VietGeeks.TestPlatform.TestManager.Api.ValidationRules;
using VietGeeks.TestPlatform.TestManager.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddVietGeeksAspNetCore(builder.Configuration.GetSection("Auth").Get<AuthOptions>() ?? new AuthOptions());
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


builder.Services.AddValidatorsFromAssemblyContaining<NewTestValidator>();
builder.Services.RegisterInfrastructureModule(builder.Configuration.GetSection("TestManagerDatabase").Get<DatabaseOptions>() ?? new DatabaseOptions());
var app = builder.Build();

app.UseCors("dev");


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
