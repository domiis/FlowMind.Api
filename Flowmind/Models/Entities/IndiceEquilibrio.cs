using Flowmind.Models;

namespace FlowMind.Api.Models.Entities
{
    public class IndiceEquilibrio
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime Data { get; set; } = DateTime.Today;
        public int Valor { get; set; }
        public string ResumoTexto { get; set; } = string.Empty;

        public User User { get; set; } = null!;
    }
}