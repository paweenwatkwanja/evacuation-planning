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

var builder = WebApplication.CreateBuilder(args);

var _connectionString = builder.Configuration.GetConnectionString("PostgresqlConnection");
builder.Services.AddDbContext<EvacuationPlanningDbContext>(options =>
    options.UseNpgsql(_connectionString));

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = ConfigurationOptions.Parse(
        builder.Configuration.GetConnectionString("RedisConnection"),
        true);

    configuration.AbortOnConnectFail = false;

    return ConnectionMultiplexer.Connect(configuration);
});

builder.Services.AddScoped<EvacuationPlanningBusinessFlow>();

builder.Services.AddSingleton<RedisService>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddControllers();

var app = builder.Build();

await using var scope = app.Services.CreateAsyncScope();
var dbContext = scope.ServiceProvider.GetRequiredService<EvacuationPlanningDbContext>();
var canConnect = await dbContext.Database.CanConnectAsync();
dbContext.Database.Migrate();
Console.WriteLine($"Database connection successful: {canConnect}");

app.UseMiddleware<GlobalExceptionHandler>();
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();