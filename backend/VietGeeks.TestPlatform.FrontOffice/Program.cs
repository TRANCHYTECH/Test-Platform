using Azure.Identity;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using VietGeeks.TestPlatform.AspNetCore;
using VietGeeks.TestPlatform.TestRunner.Api.Actors;
using VietGeeks.TestPlatform.TestRunner.Api.Filters;
using VietGeeks.TestPlatform.TestRunner.Api.Swagger;
using VietGeeks.TestPlatform.TestRunner.Infrastructure;

const string testRunnerSpaPolicy = "test-runner-spa";

var builder = WebApplication.CreateBuilder(args);

Uri vaultUri = new($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/");
Uri storageUri = new($"https://{builder.Configuration["BlobStorageName"]}.blob.core.windows.net");
builder.Configuration.AddAzureKeyVault(vaultUri, builder.Environment.GetTokenCredential());
builder.Services.AddVietGeeksAspNetCore(new VietGeeksAspNetCoreOptions());

builder.Services.AddDataProtection()
    .SetApplicationName(builder.Configuration.GetValue<string>("DataProtection:ApplicationName")!)
    .PersistKeysToAzureBlobStorage(new Uri(storageUri, $"{builder.Configuration.GetValue<string>("DataProtection:FileName")}"), builder.Environment.GetTokenCredential())
    .ProtectKeysWithAzureKeyVault(new Uri(vaultUri, $"keys/{builder.Configuration.GetValue<string>("DataProtection:KeyName")}"), builder.Environment.GetTokenCredential());

builder.Services.RegisterTestRunnerModule(new InfrastructureDataOptions
{
    Database = builder.Configuration.GetSection("TestManagerDatabase").Get<DatabaseOptions>()!
});
builder.Services.AddControllers(c => c.Filters.Add<ActorInvokeExceptionFilterAttribute>());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.CustomOperationIds(e =>
    {
        var attribute = e.CustomAttributes().FirstOrDefault(x => x.GetType() == typeof(SwaggerOperationAttribute));
        if (attribute != null)
        {
            return ((SwaggerOperationAttribute)attribute).OperationId;
        }

        return e.TryGetMethodInfo(out var methodInfo) ? methodInfo.Name : null;
    });
    c.OperationFilter<TestSessionHeaderFilter>();
});
builder.Services.AddCors(options =>
{
    options.AddPolicy(testRunnerSpaPolicy,
        policyBuilder =>
        {
            policyBuilder
                .WithOrigins(builder.Configuration.GetValue<string>("TestRunnerUrl")!)
                .AllowCredentials()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

builder.Services.AddDaprClient();
builder.Services.AddActors(options => { options.Actors.RegisterActor<ProctorActor>(); });

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

builder.Services.AddMemoryCache();

var app = builder.Build();
app.UseCors(testRunnerSpaPolicy);
app.MapActorsHandlers();
app.UseVietGeeksEssentialFeatures();
app.MapControllers();

await app.RunAsync();