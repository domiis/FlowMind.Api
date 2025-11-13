using Flowmind.Models;

namespace FlowMind.Api.Models.Entities
{
    public class RotinaDiaria
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime Data { get; set; } = DateTime.Today;
        public string RotinaJson { get; set; } = string.Empty;

        public User User { get; set; } = null!;
    }
}