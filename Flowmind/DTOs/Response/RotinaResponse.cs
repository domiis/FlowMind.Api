// DTOs/Response/RotinaResponse.cs
namespace FlowMind.Api.DTOs.Response
{
    public class RotinaResponse
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public string RotinaJson { get; set; } = string.Empty;
    }
}