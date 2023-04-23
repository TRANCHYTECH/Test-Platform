using FluentValidation;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using MongoDB.Entities;
using VietGeeks.TestPlatform.SharedKernel.PureServices;
using VietGeeks.TestPlatform.TestManager.Infrastructure.Services;
using VietGeeks.TestPlatform.TestManager.Infrastructure.Validators.TestDefintion;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static void RegisterInfrastructureModule(this IServiceCollection serviceCollection, InfrastructureDataOptions options)
    {
        ConfigureDb(options.Database);

        serviceCollection.AddScoped<ITestManagerService, TestManagerService>();
        serviceCollection.AddScoped<IQuestionManagerService, QuestionManagerService>();
        serviceCollection.AddScoped<IQuestionCategoryService, QuestionCategoryService>();
        serviceCollection.AddScoped<ITestCategoryService, TestCategoryService>();
        serviceCollection.AddScoped<ITestReportService, TestReportService>();
        serviceCollection.AddScoped<IQuestionPointCalculationService, QuestionPointCalculationService>();
        serviceCollection.AddSingleton<IClock, Clock>();
        serviceCollection.AddAutoMapper(typeof(ServiceCollectionExtensions));
        serviceCollection.AddScoped<TestManagerDbContext>();
        serviceCollection.AddAzureClients(builder => builder.AddServiceBusClient(options.ServiceBus.ConnectionString));

        serviceCollection.AddValidators();
    }

    private static void AddValidators(this IServiceCollection serviceCollection)
    {
        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
        serviceCollection.AddValidatorsFromAssemblyContaining<TestDefinitionValidator>();
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