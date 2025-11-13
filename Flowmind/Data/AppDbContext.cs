using Flowmind.Models;
using FlowMind.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlowMind.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<CheckinDiario> CheckinsDiarios { get; set; }
        public DbSet<EventoDia> EventosDias { get; set; }           // +s
        public DbSet<RotinaDiaria> RotinasDiarias { get; set; }
        public DbSet<IndiceEquilibrio> IndiceEquilibrio { get; set; } // singular
    }
}