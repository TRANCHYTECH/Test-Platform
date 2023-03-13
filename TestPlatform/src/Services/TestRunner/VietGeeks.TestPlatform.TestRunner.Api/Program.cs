using VietGeeks.TestPlaftorm.TestRunner.Infrastructure;
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

var dataOptions = new InfrastructureDataOptions
{
    Database = builder.Configuration.GetSection("TestRunnerDatabase").Get<DatabaseOptions>() ?? new DatabaseOptions()
};

builder.Services.RegisterInfrastructureModule(dataOptions);

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

app.Run();