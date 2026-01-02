using Microsoft.AspNetCore.Mvc;
using Models;
using Microsoft.EntityFrameworkCore;
using BusinessFlow;
using Repository;
using Database;
using Services;
using Exceptions;
using StackExchange.Redis;

[assembly: ApiController]

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

IWebHostEnvironment environment = builder.Environment;
Console.WriteLine($"Environment: {environment.EnvironmentName}");

Console.WriteLine("Configuring Postgresql...");

string postgresConnection = builder.Configuration.GetConnectionString("PostgresqlConnection") ?? throw new InvalidOperationException("Postgresql connection string is not configured."); 
builder.Services.AddDbContext<EvacuationPlanningDbContext>(options =>
    options.UseNpgsql(postgresConnection));

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    Console.WriteLine("Configuring Redis...");
    string redisConnection = builder.Configuration.GetConnectionString("RedisConnection") 
        ?? throw new InvalidOperationException("Redis connection string not found.");
    ConfigurationOptions configuration = ConfigurationOptions.Parse(redisConnection, true);
    configuration.AbortOnConnectFail = false;

    return ConnectionMultiplexer.Connect(configuration);
});

builder.Services.AddScoped<EvacuationPlanningBusinessFlow>();

builder.Services.AddScoped<RedisService>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddControllers();

WebApplication app = builder.Build();

await using AsyncServiceScope scope = app.Services.CreateAsyncScope();
EvacuationPlanningDbContext dbContext = scope.ServiceProvider.GetRequiredService<EvacuationPlanningDbContext>();
bool canConnect = await dbContext.Database.CanConnectAsync();
if (canConnect)
{
    IEnumerable<string> pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

    if (pendingMigrations.Any())
    {
        Console.WriteLine($"Applying {pendingMigrations.Count()} pending migration(s)...");
        await dbContext.Database.MigrateAsync();
        Console.WriteLine("Migrations applied successfully.");
    }
    else
    {
        Console.WriteLine("Database is already up to date.");
    }
}

app.UseMiddleware<GlobalExceptionHandler>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();