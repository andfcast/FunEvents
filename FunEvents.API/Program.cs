using FunEvents.API.Endpoints;
using FunEvents.API.Middlewares;
using FunEvents.Application.Interfaces;
using FunEvents.Application.Services.Implementation;
using FunEvents.Application.Services.Interfaces;
using FunEvents.Infrastructure.Persistence;
using FunEvents.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuración de Aspire (Service Defaults: OpenTelemetry, Metrics, HealthChecks)
builder.AddServiceDefaults();

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Exceptions & Middleware
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// 2. DbContext integrado con Aspire (Toma el nombre "DefaultConnection" definido en AppHost)
builder.AddNpgsqlDbContext<AppDbContext>("funeventsdb", configureDbContextOptions: options =>
{
    options.UseNpgsql(b => b.MigrationsAssembly("FunEvents.Infrastructure"));
});

// 3. Inyección de Dependencias
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IReservaService, ReservaService>();
builder.Services.AddScoped<ISeedService, SeedService>();

var app = builder.Build();

// 4. Endpoints predeterminados de Aspire (/health, /alive)
app.MapDefaultEndpoints();

// 5. Migraciones Automáticas al Iniciar
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        logger.LogInformation("Verificando y aplicando migraciones de base de datos...");
        await dbContext.Database.MigrateAsync();
        logger.LogInformation("Base de datos actualizada correctamente.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Ocurrió un error al aplicar las migraciones a la base de datos.");
        throw;
    }
}

app.UseExceptionHandler();
app.UseHttpsRedirection();

// 6. Mapeo de Endpoints
app.MapReservaEndpoints();

// Endpoint de Seed delegando a ISeedService (Clean Architecture)
app.MapPost("/api/eventos/seed", async (ISeedService seedService, CancellationToken ct) =>
{
    var resultado = await seedService.SembrarDatosPruebaAsync(ct);
    return Results.Ok(new { Mensaje = resultado });
});

app.Run();