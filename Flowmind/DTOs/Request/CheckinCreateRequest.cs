// DTOs/Request/CheckinCreateRequest.cs
using FlowMind.Api.Models.Entities;
namespace FlowMind.Api.DTOs.Request
{
    public class CheckinCreateRequest
    {
        public Humor Humor { get; set; }
        public Energia Energia { get; set; }
        public Sono Sono { get; set; }
    }
}