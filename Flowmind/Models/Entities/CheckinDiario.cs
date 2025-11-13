// Models/Entities/CheckinDiario.cs
using Flowmind.Models;

namespace FlowMind.Api.Models.Entities
{
    public class CheckinDiario
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime Data { get; set; }
        public Humor Humor { get; set; }
        public Energia Energia { get; set; }
        public Sono Sono { get; set; }

        public User User { get; set; } = null!;
    }

    public enum Humor { Ansioso, Neutro, Motivado, Estressado, Calmo }
    public enum Energia { Baixa, Media, Alta }
    public enum Sono { Ruim, Ok, Bom }
}