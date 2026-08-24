using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Domain.Entities
{
    public class Reserva
    {
        public Guid Id { get; private set; }
        public Guid EventoId { get; private set; }
        public string UsuarioId { get; private set; }
        public int Cantidad { get; private set; }
        public DateTime FechaReserva { get; private set; }

        private Reserva() { }

        internal Reserva(Guid eventoId, string usuarioId, int cantidad)
        {
            Id = Guid.NewGuid();
            EventoId = eventoId;
            UsuarioId = usuarioId;
            Cantidad = cantidad;
            FechaReserva = DateTime.UtcNow;
        }
    }
}
