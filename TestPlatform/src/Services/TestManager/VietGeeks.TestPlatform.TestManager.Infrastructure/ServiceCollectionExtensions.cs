using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDB.Entities;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static void RegisterInfrastructureModule(this IServiceCollection serviceCollection, DatabaseOptions databaseOptions)
    {
        DB.InitAsync(databaseOptions.DatabaseName, MongoClientSettings.FromConnectionString(databaseOptions.ConnectionString)).Wait();

        serviceCollection.AddScoped<ITestManagerService, TestManagerService>();
        serviceCollection.AddAutoMapper(typeof(ServiceCollectionExtensions));
        serviceCollection.AddScoped<TestManagerDbContext>();
    }
}

public class DatabaseOptions
{
    public string DatabaseName { get; set; } = default!;

    public string ConnectionString { get; set; } = default!;
}