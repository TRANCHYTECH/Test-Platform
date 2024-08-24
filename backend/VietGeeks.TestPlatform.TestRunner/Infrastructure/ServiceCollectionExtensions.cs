using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using MongoDB.Entities;
using VietGeeks.TestPlatform.TestRunner.Infrastructure.Services;

namespace VietGeeks.TestPlatform.TestRunner.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static void RegisterTestRunnerModule(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        var databaseOptions = new DatabaseOptions
        {
            ConnectionString = configuration.GetConnectionString("TestManager")!,
            DatabaseName = configuration["TestManagerDatabaseName"]!,
        };
        ConfigureDb(databaseOptions);

        serviceCollection.AddScoped<IProctorService, ProctorService>();
        serviceCollection.AddScoped<TestRunnerDbContext>();
        serviceCollection.AddAutoMapper(typeof(ServiceCollectionExtensions));
    }

    private static void ConfigureDb(DatabaseOptions databaseOptions)
    {
        var conventionPack = new ConventionPack
        {
            new IgnoreExtraElementsConvention(ignoreExtraElements: true),
        };

        ConventionRegistry.Register("TestPlatformDefaultConventions", conventionPack, _ => true);

        DB.InitAsync(databaseOptions.DatabaseName,
            MongoClientSettings.FromConnectionString(databaseOptions.ConnectionString)).Wait();
    }
}

public class DatabaseOptions
{
    public string DatabaseName { get; set; } = default!;

    public string ConnectionString { get; set; } = default!;
}

public class ServiceBusOptions
{
    public string ConnectionString { get; set; } = default!;
}

public class InfrastructureDataOptions
{
    public DatabaseOptions Database { get; set; } = default!;

    public ServiceBusOptions ServiceBus { get; set; } = default!;
}