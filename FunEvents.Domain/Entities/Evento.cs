using FunEvents.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Domain.Entities
{
    public class Evento
    {
        public Guid Id { get; private set; }
        public string Nombre { get; private set; } = string.Empty;
        public int EntradasDisponibles { get; private set; }

        private Evento() { }

        public Evento(Guid id, string nombre, int entradasDisponibles)
        {
            Id = id;
            Nombre = nombre;
            EntradasDisponibles = entradasDisponibles;
        }

        public Reserva CrearReserva(string usuarioId, int cantidad)
        {
            if (cantidad <= 0)
                throw new DomainException("La cantidad de entradas debe ser mayor a cero.");

            if (EntradasDisponibles < cantidad)
                throw new DomainException($"No hay suficientes entradas disponibles. Disponibles: {EntradasDisponibles}.");

            EntradasDisponibles -= cantidad;
            return new Reserva(Id, usuarioId, cantidad);
        }
    }
}
