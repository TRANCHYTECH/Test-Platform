﻿using System.IdentityModel.Tokens.Jwt;
using Duende.Bff;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VietGeeks.TestPlatform.AspNetCore.Services;
using VietGeeks.TestPlatform.SharedKernel;
using VietGeeks.TestPlatform.SharedKernel.Exceptions;

namespace VietGeeks.TestPlatform.AspNetCore;

public static class ServiceCollectionExtensions
{
    public static void AddVietGeeksAspNetCore(this IServiceCollection serviceCollection,
        VietGeeksAspNetCoreOptions options)
    {
        serviceCollection.Configure<ForwardedHeadersOptions>(options =>
            options.ForwardedHeaders = ForwardedHeaders.XForwardedHost | ForwardedHeaders.XForwardedProto);

        serviceCollection.AddSingleton<IPostConfigureOptions<CookieAuthenticationOptions>, ForcePostConfigureApplicationCookieTicketStore>();
        if (options.Auth != null)
        {
            var authOption = options.Auth;
            serviceCollection.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = authOption.Authority;
                options.Audience = authOption.Audience;
            });
        }

        if (options.OpenIdConnect != null)
        {
            var openIdConnectSettings = options.OpenIdConnect;
            serviceCollection.AddAuthentication(configure =>
                {
                    configure.DefaultScheme = AuthenticationSchemaNames.MultiAuthSchemes;
                    configure.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                    configure.DefaultSignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
                }).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Cookie.Name = "__Host.BackOffice";
                    options.Cookie.SameSite = SameSiteMode.Strict;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                }).AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, configure =>
                {
                    configure.Authority = openIdConnectSettings.Authority;
                    // confidential client using code flow + PKCE
                    configure.ClientId = openIdConnectSettings.ClientId;
                    configure.ClientSecret = openIdConnectSettings.ClientSecret;
                    configure.ResponseType = "code";
                    configure.ResponseMode = "query";
                    configure.MapInboundClaims = false;
                    configure.GetClaimsFromUserInfoEndpoint = true;
                    configure.SaveTokens = true;
                    // request scopes + refresh tokens
                    configure.Scope.Clear();
                    foreach (var scope in openIdConnectSettings.Scopes)
                    {
                        configure.Scope.Add(scope);
                    }

                    configure.Events = new OpenIdConnectEvents
                    {
                        OnRedirectToIdentityProvider = context =>
                        {
                            context.ProtocolMessage.SetParameter("audience",
                                openIdConnectSettings.ValidAudiences[0]);
                            return Task.FromResult(0);
                        },
                        OnRedirectToIdentityProviderForSignOut = context =>
                        {
                            var logoutUri =
                                $"{openIdConnectSettings.Authority}/v2/logout?client_id={openIdConnectSettings.ClientId}";

                            var postLogoutUri = context.Properties.RedirectUri;
                            if (!string.IsNullOrEmpty(postLogoutUri))
                            {
                                if (string.Equals(postLogoutUri, "/test-portal", StringComparison.Ordinal))
                                {
                                    postLogoutUri = "/test-portal";
                                }

                                logoutUri += $"&returnTo={Uri.EscapeDataString(postLogoutUri)}";
                            }

                            context.Response.Redirect(logoutUri);
                            context.HandleResponse();

                            return Task.CompletedTask;
                        }
                    };
                })
                .AddJwtBearer(AuthenticationSchemaNames.UserSynchronization)
                .AddPolicyScheme(AuthenticationSchemaNames.MultiAuthSchemes, "Multi Auth Schemes", configure =>
                {
                    configure.ForwardDefaultSelector = context =>
                    {
                        // For advanced usage, we could read token and check audience, then redirect to corresponding schema
                        string? authorization = context.Request.Headers.Authorization;
                        const string bearerPrefix = "Bearer ";
                        if (!string.IsNullOrEmpty(authorization) &&
                            authorization.StartsWith(bearerPrefix, StringComparison.Ordinal))
                        {
                            var token = authorization[bearerPrefix.Length..];
                            var handler = new JwtSecurityTokenHandler();
                            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

                            if (jsonToken?.Audiences.Contains("backoffice-api/auth0", StringComparer.Ordinal) ==
                                true)
                            {
                                return AuthenticationSchemaNames.UserSynchronization;
                            }
                        }

                        return CookieAuthenticationDefaults.AuthenticationScheme;
                    };
                });

            serviceCollection.AddAuthorizationBuilder()
                .AddPolicy(AuthPolicyNames.CreateUserPolicy, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "write:user");
                });
        }

        serviceCollection.AddHttpContextAccessor();
        serviceCollection.AddScoped<ITenant, Tenant>();
    }

    public static void UseVietGeeksEssentialFeatures(this WebApplication app)
    {
        app.UseForwardedHeaders();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseExceptionHandler(configure => configure.Run(async context =>
        {
            var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
            if (exceptionHandlerPathFeature?.Error is TestPlatformException ex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new ErrorDetails { Error = ex.Message });
            }
        }));
    }
}

public class VietGeeksAspNetCoreOptions
{
    public DataProtectionOptions? DataProtection { get; init; }

    public AuthOptions? Auth { get; set; }

    public OpenIdConnectOptions? OpenIdConnect { get; init; }
}

public class AuthOptions
{
    public string Authority { get; set; } = default!;

    public string Audience { get; set; } = default!;
}

public class OpenIdConnectOptions
{
    public string Authority { get; init; } = default!;

    public string ClientId { get; init; } = default!;

    public string ClientSecret { get; init; } = default!;

    public string[] ValidAudiences { get; init; } = default!;

    public string[] Scopes { get; init; } = default!;

    public string? AuthorizationUrl { get; init; }

    public string? TokenUrl { get; init; }
}

public class ErrorDetails
{
    public string Error { get; set; } = default!;
}

public class ForcePostConfigureApplicationCookieTicketStore : IPostConfigureOptions<CookieAuthenticationOptions>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="httpContextAccessor"></param>
    public ForcePostConfigureApplicationCookieTicketStore(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

    /// <inheritdoc />
    public void PostConfigure(string? name, CookieAuthenticationOptions options) => options.SessionStore = new TicketStoreShim(_httpContextAccessor);
}
