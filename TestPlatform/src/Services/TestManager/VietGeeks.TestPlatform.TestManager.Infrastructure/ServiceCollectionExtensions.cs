using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Entities;
using VietGeeks.TestPlatform.TestManager.Infrastructure.Services;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static void RegisterInfrastructureModule(this IServiceCollection serviceCollection, DatabaseOptions databaseOptions)
    {
        DB.InitAsync(databaseOptions.DatabaseName, MongoClientSettings.FromConnectionString(databaseOptions.ConnectionString)).Wait();

        serviceCollection.AddScoped<ITestManagerService, TestManagerService>();
        serviceCollection.AddScoped<IQuestionManagerService, QuestionManagerService>();
        serviceCollection.AddScoped<IQuestionCategoryService, QuestionCategoryService>();
        serviceCollection.AddAutoMapper(typeof(ServiceCollectionExtensions));
        serviceCollection.AddScoped<TestManagerDbContext>();
    }
}

public class DatabaseOptions
{
    public string DatabaseName { get; set; } = default!;

    public string ConnectionString { get; set; } = default!;
}