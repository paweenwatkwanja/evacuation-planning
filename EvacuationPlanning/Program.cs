using Microsoft.AspNetCore.Mvc;
using Models;
using Microsoft.EntityFrameworkCore;
using BusinessFlow;
using Repository;
using Database;

[assembly: ApiController]

var builder = WebApplication.CreateBuilder(args);

var _connectionString = builder.Configuration.GetConnectionString("PostgresqlConnection");
builder.Services.AddDbContext<EvacuationPlanningDbContext>(options =>
    options.UseNpgsql(_connectionString));

builder.Services.AddScoped<EvacuationPlanningBusinessFlow>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddControllers();

var app = builder.Build();

await using var scope = app.Services.CreateAsyncScope();
var dbContext = scope.ServiceProvider.GetRequiredService<EvacuationPlanningDbContext>();
var canConnect = await dbContext.Database.CanConnectAsync();
dbContext.Database.Migrate();
Console.WriteLine($"Database connection successful: {canConnect}");

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.Run();