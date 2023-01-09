using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;

namespace VietGeeks.TestPlatform.AspNetCore
{
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
    }

    public class AuthOptions
    {
        public string Authority { get; set; } = default!;

        public string Audience { get; set; } = default!;
    }
}

