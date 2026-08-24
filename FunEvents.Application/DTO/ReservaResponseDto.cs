using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Application.DTO
{
    public record ReservaResponseDto(Guid ReservaId, string Mensaje, DateTime FechaReserva);
}
