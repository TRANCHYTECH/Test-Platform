using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using MongoDB.Entities;
using VietGeeks.TestPlatform.AccountManager.Services;
using VietGeeks.TestPlatform.SharedKernel;
using VietGeeks.TestPlatform.SharedKernel.PureServices;

namespace VietGeeks.TestPlatform.AccountManager;

public static class ServiceCollectionExtensions
{
    public static void RegisterAccountManagerModule(this IServiceCollection serviceCollection, AccountManagerModuleOptions options)
    {
        ConfigureDb(options.Database);

        serviceCollection.AddScoped<IAccountSettingsService, AccountSettingsService>();
        serviceCollection.AddSingleton<IClock, Clock>();
        serviceCollection.AddAutoMapper(typeof(ServiceCollectionExtensions));
    }

    private static void ConfigureDb(DatabaseOptions databaseOptions)
    {
        var conventionPack = new ConventionPack
        {
            new IgnoreExtraElementsConvention(true)
        };

        ConventionRegistry.Register("TestPlatformDefaultConventions", conventionPack, _ => true);

        DB.InitAsync(databaseOptions.DatabaseName, MongoClientSettings.FromConnectionString(databaseOptions.ConnectionString)).Wait();
    }
}

public class AccountManagerModuleOptions
{
    public required DatabaseOptions Database { get; init; }
}
