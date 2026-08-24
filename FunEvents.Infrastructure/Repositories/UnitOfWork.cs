using FunEvents.Application.Interfaces;
using FunEvents.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IEventoRepository Eventos { get; }
        public IReservaRepository Reservas { get; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Eventos = new EventoRepository(_context);
            Reservas = new ReservaRepository(_context);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => await _context.SaveChangesAsync(cancellationToken);
    }
}
