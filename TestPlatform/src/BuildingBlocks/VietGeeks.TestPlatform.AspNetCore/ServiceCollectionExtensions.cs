using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using VietGeeks.TestPlatform.SharedKernel.Exceptions;

namespace VietGeeks.TestPlatform.AspNetCore;

public static class ServiceCollectionExtensions
{
    public static void AddVietGeeksAspNetCore(this IServiceCollection serviceCollection, AuthOptions authOptions)
    {
        serviceCollection.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.Authority = authOptions.Authority;
            options.Audience = authOptions.Audience;
        });

        serviceCollection.AddHttpContextAccessor();
        serviceCollection.AddScoped<ITenant, Tenant>();
    }

    public static void UseVietGeeksEssentialFeatures(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseExceptionHandler(configure => configure.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                if (exceptionHandlerPathFeature?.Error is TestPlatformException ex)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsJsonAsync<ErrorDetails>(new() { Error = ex.Message });
                }
            }));
    }
}

public class AuthOptions
{
    public string Authority { get; set; } = default!;

    public string Audience { get; set; } = default!;
}

public class ErrorDetails
{
    public string Error { get; set; } = default!;
}
