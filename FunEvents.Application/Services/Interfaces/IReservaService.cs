using FunEvents.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Application.Services.Interfaces
{
    public interface IReservaService
    {
        Task<ReservaResponseDto> ProcesarReservaAsync(CrearReservaDto request, CancellationToken cancellationToken);
    }
}
