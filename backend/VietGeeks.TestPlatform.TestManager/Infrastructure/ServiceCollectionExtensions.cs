using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using MongoDB.Entities;
using VietGeeks.TestPlatform.SharedKernel;
using VietGeeks.TestPlatform.SharedKernel.PureServices;
using VietGeeks.TestPlatform.TestManager.Infrastructure.Services;
using VietGeeks.TestPlatform.TestManager.Infrastructure.Validators.TestDefinition;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static void RegisterTestManagerModule(this IServiceCollection serviceCollection,
        TestManagerModuleOptions options)
    {
        ConfigureDb(options.Database);

        serviceCollection.AddScoped<ITestManagerService, TestManagerService>();
        serviceCollection.AddScoped<IQuestionManagerService, QuestionManagerService>();
        serviceCollection.AddScoped<IQuestionCategoryService, QuestionCategoryService>();
        serviceCollection.AddScoped<ITestCategoryService, TestCategoryService>();
        serviceCollection.AddScoped<ITestReportService, TestReportService>();
        serviceCollection.AddScoped<IQuestionPointCalculationService, QuestionPointCalculationService>();
        serviceCollection.AddScoped<IQuestionRelatedValidationService, QuestionRelatedValidationService>();
        serviceCollection.AddSingleton<IClock, Clock>();
        serviceCollection.AddAutoMapper(typeof(ServiceCollectionExtensions));
        serviceCollection.AddScoped<TestManagerDbContext>();
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

        var settings = MongoClientSettings.FromConnectionString(databaseOptions.ConnectionString);
        settings.UseTls = true;
        DB.InitAsync(databaseOptions.DatabaseName, settings).Wait();
    }
}

public class TestManagerModuleOptions
{
    public required DatabaseOptions Database { get; init; }
}