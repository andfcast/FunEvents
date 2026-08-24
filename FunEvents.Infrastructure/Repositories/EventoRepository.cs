using FunEvents.Application.Interfaces;
using FunEvents.Domain.Entities;
using FunEvents.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Infrastructure.Repositories
{
    public class EventoRepository : Repository<Evento>, IEventoRepository
    {
        public EventoRepository(AppDbContext context) : base(context) { }
    }
}
