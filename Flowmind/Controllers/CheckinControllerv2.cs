// Controllers/v2/CheckinController.cs
using FlowMind.Api.DTOs.Links;
using FlowMind.Api.DTOs.Request;
using FlowMind.Api.DTOs.Response;
using FlowMind.Api.Helpers;
using FlowMind.Api.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FlowMind.Api.Controllers.v2
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/checkin")]
    public class CheckinController : ControllerBase
    {
        private static readonly List<CheckinResponse> Checkins = new()
        {
            new CheckinResponse { Id = 1, Humor = Humor.Calmo, Energia = Energia.Media, Sono = Sono.Bom, Data = DateTime.Today.AddDays(-1) },
            new CheckinResponse { Id = 2, Humor = Humor.Motivado, Energia = Energia.Alta, Sono = Sono.Bom, Data = DateTime.Today }
        };

        private readonly IUrlHelper _urlHelper;

        public CheckinController(IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        [HttpGet(Name = "GetCheckinsV2")]
        public ActionResult<object> GetCheckins([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var total = Checkins.Count;
            var items = Checkins
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(EnriquecerComIndiceELinks)
                .ToList();

            return Ok(PaginationHelper.CreatePaginatedResponse(
                items, page, pageSize, total, _urlHelper, "GetCheckinsV2"));
        }

        [HttpGet("{id}", Name = "GetCheckinByIdV2")]
        public ActionResult<object> GetCheckinById(int id)
        {
            var checkin = Checkins.FirstOrDefault(c => c.Id == id);
            if (checkin == null)
                return NotFound(new { message = "Check-in não encontrado." });

            return Ok(EnriquecerComIndiceELinks(checkin));
        }

        [HttpPost(Name = "PostCheckinV2")]
        public IActionResult PostCheckin([FromBody] CheckinCreateRequest request)
        {
            var novo = new CheckinResponse
            {
                Id = Checkins.Max(c => c.Id) + 1,
                Humor = request.Humor,
                Energia = request.Energia,
                Sono = request.Sono,
                Data = DateTime.Today
            };

            Checkins.Add(novo);
            return CreatedAtRoute("GetCheckinByIdV2", new { id = novo.Id }, EnriquecerComIndiceELinks(novo));
        }

        [HttpPut("{id}", Name = "PutCheckinV2")]
        public IActionResult PutCheckin(int id, [FromBody] CheckinCreateRequest request)
        {
            var checkin = Checkins.FirstOrDefault(c => c.Id == id);
            if (checkin == null)
                return NotFound(new { message = "Check-in não encontrado." });

            checkin.Humor = request.Humor;
            checkin.Energia = request.Energia;
            checkin.Sono = request.Sono;
            checkin.Data = DateTime.Today;

            return Ok(EnriquecerComIndiceELinks(checkin));
        }

        [HttpDelete("{id}", Name = "DeleteCheckinV2")]
        public IActionResult DeleteCheckin(int id)
        {
            var checkin = Checkins.FirstOrDefault(c => c.Id == id);
            if (checkin == null)
                return NotFound(new { message = "Check-in não encontrado." });

            Checkins.Remove(checkin);
            return NoContent();
        }

        private object EnriquecerComIndiceELinks(CheckinResponse c)
        {
            c.Links.Clear();
            c.Links.AddRange(GerarLinks(c.Id));

            return new
            {
                c.Id,
                c.Humor,
                c.Energia,
                c.Sono,
                IndiceEquilibrio = CalcularIndiceEquilibrio(c),
                c.Data,
                c.Links
            };
        }

        private static int CalcularIndiceEquilibrio(CheckinResponse req)
        {
            int score = 0;
            if (req.Humor == Humor.Motivado || req.Humor == Humor.Calmo) score += 40;
            if (req.Energia == Energia.Alta) score += 30;
            if (req.Sono == Sono.Bom) score += 30;
            return Math.Min(score, 100);
        }

        private List<LinkDto> GerarLinks(int id)
        {
            return new List<LinkDto>
            {
                new() { Href = _urlHelper.Link("GetCheckinByIdV2", new { id })!, Rel = "self", Method = "GET" },
                new() { Href = _urlHelper.Link("GetCheckinsV2", new { })!, Rel = "list", Method = "GET" },
                new() { Href = _urlHelper.Link("PostCheckinV2", new { })!, Rel = "create", Method = "POST" },
                new() { Href = _urlHelper.Link("PutCheckinV2", new { id })!, Rel = "update", Method = "PUT" },
                new() { Href = _urlHelper.Link("DeleteCheckinV2", new { id })!, Rel = "delete", Method = "DELETE" }
            };
        }
    }
}