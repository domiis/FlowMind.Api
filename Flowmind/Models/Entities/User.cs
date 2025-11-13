using System.ComponentModel.DataAnnotations;
using FlowMind.Api.Models.Entities;

namespace Flowmind.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // 🔹 Relacionamentos (um-para-muitos)
        public ICollection<CheckinDiario> CheckinsDiarios { get; set; } = new List<CheckinDiario>();
        public ICollection<EventoDia> EventosDia { get; set; } = new List<EventoDia>();
        public ICollection<RotinaDiaria> RotinasDiarias { get; set; } = new List<RotinaDiaria>();
        public ICollection<IndiceEquilibrio> IndicesEquilibrio { get; set; } = new List<IndiceEquilibrio>();
    }
}
