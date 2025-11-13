// DTOs/Links/LinkDto.cs
namespace FlowMind.Api.DTOs.Links
{
    public class LinkDto
    {
        public string Href { get; set; } = string.Empty;
        public string Rel { get; set; } = string.Empty;
        public string Method { get; set; } = "GET";
    }
}