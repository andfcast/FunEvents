using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Application.Services.Interfaces
{
    public interface ISeedService
    {
        Task<string> SembrarDatosPruebaAsync(CancellationToken cancellationToken = default);
    }
}
