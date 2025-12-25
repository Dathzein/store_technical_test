using Serilog;
using ServerCloudStore.API.Extensions;
using ServerCloudStore.API.Hubs;
using ServerCloudStore.API.Middleware;
using ServerCloudStore.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/servercloudstore-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add SignalR
builder.Services.AddSignalR();

// Add JWT Authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

// Add Authorization Policies
builder.Services.AddAuthorizationPolicies();

// Add Application and Infrastructure services
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var dbInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
        await dbInitializer.InitializeAsync();
        logger.LogInformation("Scripts SQL ejecutados correctamente.");

        // Ejecutar seeding de usuarios (después de que los repositorios estén registrados)
        try
        {
            var dbSeeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
            await dbSeeder.SeedAsync();
            logger.LogInformation("Seeding de usuarios completado.");
        }
        catch (Exception seedEx)
        {
            logger.LogWarning(seedEx, "Error al ejecutar seeding. Puede que los datos ya existan.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error al inicializar la base de datos.");
        // Continuar aunque falle la inicialización (puede que ya esté inicializada)
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Exception handling middleware debe ir antes de UseAuthentication
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Map SignalR Hub
app.MapHub<ImportNotificationHub>("/hubs/import");

app.Run();

// Make the implicit Program class public for integration tests
public partial class Program { }
