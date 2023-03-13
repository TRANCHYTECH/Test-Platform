using System;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using MongoDB.Entities;
using VietGeeks.TestPlaftorm.TestRunner.Infrastructure.Services;

namespace VietGeeks.TestPlaftorm.TestRunner.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static void RegisterInfrastructureModule(this IServiceCollection serviceCollection, InfrastructureDataOptions options)
    {
        ConfigureDb(options.Database);

        serviceCollection.AddScoped<IProctorService, ProctorService>();
        //serviceCollection.AddScoped<IQuestionManagerService, QuestionManagerService>();
        //serviceCollection.AddScoped<IQuestionCategoryService, QuestionCategoryService>();
        //serviceCollection.AddScoped<IQuestionPointCalculationService, QuestionPointCalculationService>();
        //serviceCollection.AddAutoMapper(typeof(ServiceCollectionExtensions));
        serviceCollection.AddScoped<DBContext>();

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

