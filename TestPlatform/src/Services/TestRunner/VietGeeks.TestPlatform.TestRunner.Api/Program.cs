using VietGeeks.TestPlaftorm.TestRunner.Infrastructure;
using VietGeeks.TestPlatform.AspNetCore;
using VietGeeks.TestPlatform.TestRunner.Api.Actors;
using VietGeeks.TestPlatform.TestRunner.Api.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.OperationFilter<TestSessionHeaderFilter>());
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "dev",
                      builder =>
                      {
                          builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                      });
});

var daprHttpPort = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT") ?? "3600";
var daprGrpcPort = Environment.GetEnvironmentVariable("DAPR_GRPC_PORT") ?? "60000";
builder.Services.AddDaprClient(builder => builder
    .UseHttpEndpoint($"http://localhost:{daprHttpPort}")
    .UseGrpcEndpoint($"http://localhost:{daprGrpcPort}"));

builder.Services.AddActors(options =>
{
    options.Actors.RegisterActor<ProctorActor>();
});

var dataOptions = new InfrastructureDataOptions
{
    Database = builder.Configuration.GetSection("TestRunnerDatabase").Get<DatabaseOptions>() ?? new DatabaseOptions()
};

builder.Services.RegisterInfrastructureModule(dataOptions);

var app = builder.Build();

app.UseCors("dev");

app.MapActorsHandlers();

app.UseVietGeeksEssentialFeatures();

app.UseAuthorization();

app.MapControllers();

app.Run();