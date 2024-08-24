using MassTransit;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Entities;
using VietGeeks.TestPlatform.AspNetCore;
using VietGeeks.TestPlatform.TestManager.Infrastructure.EventConsumers;

namespace VietGeeks.TestPlatform.BackOffice
{
    public static class HostBuilderExtensions
    {
        public static void AddTestManagerMassTransit(this WebApplicationBuilder builder)
        {
            builder.Services.AddMassTransit(c =>
            {
                c.ConfigureHealthCheckOptions(cfg => cfg.MinimalFailureStatus = HealthStatus.Degraded);
                c.AddMongoDbOutbox(o =>
                {
                    var dbName  = builder.Configuration.GetValue<string>("TestManagerDatabaseName");
                    o.ClientFactory(_ => DB.Database(dbName).Client);
                    o.DatabaseFactory(_ => DB.Database(dbName));
                    o.DuplicateDetectionWindow = TimeSpan.FromSeconds(30);
                    o.UseBusOutbox();
                });

                var endpointPrefix = builder.Environment.IsDevelopment() ? Environment.UserName : string.Empty;
                c.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter(endpointPrefix, includeNamespace: false));
                c.AddConsumersFromNamespaceContaining<SendTestAccessCode>();
                c.UsingAzureServiceBus((ctx, factoryConfig) =>
                {
                    factoryConfig.Host(builder.Configuration.GetConnectionString("ServiceBus"),
                        hostConfig => hostConfig.TokenCredential = builder.Environment.GetTokenCredential());
                    factoryConfig.ConfigureEndpoints(ctx);
                });
            });
        }
    }
}