using FunEvents.Application.Interfaces;
using FunEvents.Domain.Entities;
using FunEvents.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Infrastructure.Repositories
{
    public class ReservaRepository : Repository<Reserva>, IReservaRepository
    {
        public ReservaRepository(AppDbContext context) : base(context) { }
    }
}
