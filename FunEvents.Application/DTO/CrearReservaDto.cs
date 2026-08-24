using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Application.DTO
{
    public record CrearReservaDto(Guid Id, Guid EventoId, string UsuarioId, int Cantidad);        
}
