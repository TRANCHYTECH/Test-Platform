using Dapr.Client.Autogen.Grpc.v1;
using Microsoft.Extensions.DependencyInjection;
using VietGeeks.TestPlatform.TestRunner.Api.Actors;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var daprHttpPort = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT") ?? "3600";
var daprGrpcPort = Environment.GetEnvironmentVariable("DAPR_GRPC_PORT") ?? "60000";
builder.Services.AddDaprClient(builder => builder
    .UseHttpEndpoint($"http://localhost:{daprHttpPort}")
    .UseGrpcEndpoint($"http://localhost:{daprGrpcPort}"));

builder.Services.AddActors(options =>
{
    options.Actors.RegisterActor<ProctorActor>();
});

var app = builder.Build();

app.MapActorsHandlers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.MapGet("/test/start-mininal/{code}", (string code) =>
{
    return code;
});

app.Run("http://localhost:6000");

//dapr run --app-id testrunner --app-port 6000 --dapr-http-port 3600 --dapr-grpc-port 60000 dotnet run -p VietGeeks.TestPlatform.TestRunner.Api --urls 
// dapr run --app-id trafficcontrolservice --app-port 6000 --dapr-http-port 3600 --dapr-grpc-port 60000 --config ../dapr/config/config.yaml --components-path ../dapr/components dotnet run