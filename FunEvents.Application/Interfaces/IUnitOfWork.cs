using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IEventoRepository Eventos { get; }
        IReservaRepository Reservas { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
