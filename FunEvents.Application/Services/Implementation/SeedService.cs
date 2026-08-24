using FunEvents.Application.Interfaces;
using FunEvents.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Application.Services.Implementation
{
    public class SeedService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SeedService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> SembrarDatosPruebaAsync(CancellationToken cancellationToken = default)
        {
            var eventoId = Guid.Parse("11111111-1111-1111-1111-111111111111");

            // Usamos el repositorio expuesto directamente por la UnitOfWork
            var eventoExistente = await _unitOfWork.Eventos.GetByIdAsync(eventoId, cancellationToken);

            if (eventoExistente is null)
            {
                var eventoPrueba = new Evento(eventoId, "Concierto de Rock FunEvents 2026", 100);

                await _unitOfWork.Eventos.AddAsync(eventoPrueba, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return "Evento de prueba creado con éxito.";
            }

            return "El evento de prueba ya existe en la base de datos.";
        }
    }
}
