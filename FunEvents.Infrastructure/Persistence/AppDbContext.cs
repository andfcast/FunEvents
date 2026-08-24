using FunEvents.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Evento> Eventos => Set<Evento>();
        public DbSet<Reserva> Reservas => Set<Reserva>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Evento>(builder =>
            {
                builder.HasKey(e => e.Id);
                builder.Property(e => e.Nombre).IsRequired().HasMaxLength(150);
                builder.Property(e => e.EntradasDisponibles).IsRequired();
            });

            modelBuilder.Entity<Reserva>(builder =>
            {
                builder.HasKey(r => r.Id);
                builder.Property(r => r.UsuarioId).IsRequired().HasMaxLength(100);
            });
        }
    }
}
