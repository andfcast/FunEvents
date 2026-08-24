using FunEvents.Application.DTO;
using FunEvents.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FunEvents.API.Endpoints
{
    public static class ReservaEndpoints
    {
        // Método de extensión para mapear las Minimal APIs de este módulo
        public static void MapReservaEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/reservas");

            group.MapPost("/", async (CrearReservaDto request, IReservaService reservaService, CancellationToken cancellationToken) =>
            {
                var resultado = await reservaService.ProcesarReservaAsync(request, cancellationToken);
                return Results.Created($"/api/reservas/{resultado.ReservaId}", resultado);
            })
            .WithName("CrearReserva")
            .Produces<ReservaResponseDto>(StatusCodes.Status201Created)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces<string>(StatusCodes.Status404NotFound);

            group.MapPost("/eventos/seed", async (ISeedService seedService, CancellationToken cancellationToken) =>
            {
                var resultado = await seedService.SembrarDatosPruebaAsync(cancellationToken);
                return Results.Ok(new { Mensaje = resultado });
            });
        }
    }
}
