using FunEvents.API.Endpoints;
using FunEvents.API.Middlewares;
using FunEvents.Application.Interfaces;
using FunEvents.Application.Services.Implementation;
using FunEvents.Application.Services.Interfaces;
using FunEvents.Infrastructure.Persistence;
using FunEvents.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//Enable logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no está configurada en appsettings.json.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString, b =>
        b.MigrationsAssembly("FunEvents.Infrastructure")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IReservaService, ReservaService>();

var app = builder.Build();

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
        throw; // Importante para detener la app si la base de datos no está lista
    }
}

app.UseExceptionHandler();

app.MapReservaEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/api/eventos/seed", async (AppDbContext db, CancellationToken ct) =>
{
    if (!await db.Eventos.AnyAsync(ct))
    {
        var evento = new FunEvents.Domain.Entities.Evento(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            "Concierto de Rock FunEvents",
            50
        );
        db.Eventos.Add(evento);
        await db.SaveChangesAsync(ct);
    }
    return Results.Ok("Datos iniciales cargados exitosamente.");
});

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.Run();
