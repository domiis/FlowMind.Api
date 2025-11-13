using Flowmind.Models;

namespace FlowMind.Api.Models.Entities
{
    public class EventoDia
    {
        public int Id { get; set; }
        public int UserId { get; set; }   // 🔹 alterado de IdUsuario
        public DateTime Data { get; set; }
        public TipoEvento TipoEvento { get; set; }

        public User User { get; set; } = null!;
    }

    public enum TipoEvento
    {
        Pausa,
        Crise,
        Recompensa
    }
}
