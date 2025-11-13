// DTOs/Response/CheckinResponse.cs
using FlowMind.Api.DTOs.Links;
using FlowMind.Api.Models.Entities;

namespace FlowMind.Api.DTOs.Response
{
    public class CheckinResponse
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public Humor Humor { get; set; }
        public Energia Energia { get; set; }
        public Sono Sono { get; set; }

        public List<LinkDto> Links { get; set; } = new();
    }
}