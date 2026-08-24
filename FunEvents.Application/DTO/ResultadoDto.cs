using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Application.DTO
{
    public record ResultadoDto<T>(bool EsExitoso, T? Data, string? Error);
}
