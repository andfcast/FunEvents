using FunEvents.Application.DTO;
using FunEvents.Application.Interfaces;
using FunEvents.Application.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Application.Services.Implementation
{
    public class ReservaService : IReservaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ReservaService> _logger;

        public ReservaService(IUnitOfWork unitOfWork, ILogger<ReservaService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ReservaResponseDto> ProcesarReservaAsync(CrearReservaDto request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando procesamiento de reserva. EventoId: {EventoId}, UsuarioId: {UsuarioId}, Cantidad: {Cantidad}",
                request.EventoId, request.UsuarioId, request.Cantidad);

            var evento = await _unitOfWork.Eventos.GetByIdAsync(request.EventoId, cancellationToken);
            if (evento == null)
            {
                _logger.LogWarning("Intento de reserva fallido: Evento {EventoId} no encontrado.", request.EventoId);
                throw new KeyNotFoundException($"El evento con ID '{request.EventoId}' no existe.");
            }

            // Si la validación falla, lanzará DomainException
            var reserva = evento.CrearReserva(request.UsuarioId, request.Cantidad);

            _unitOfWork.Eventos.Update(evento);
            await _unitOfWork.Reservas.AddAsync(reserva, cancellationToken);

            // Pasamos el cancellationToken a la persistencia en BD
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Reserva creada con éxito. ReservaId: {ReservaId} para UsuarioId: {UsuarioId}",
                reserva.Id, request.UsuarioId);

            return new ReservaResponseDto(reserva.Id, "Reserva realizada exitosamente.", reserva.FechaReserva);
        }
    }
}
