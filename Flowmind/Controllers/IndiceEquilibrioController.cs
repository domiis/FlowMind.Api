// Controllers/IndiceEquilibrioController.cs
using FlowMind.Api.Data;
using FlowMind.Api.DTOs.Links;
using FlowMind.Api.Helpers;
using FlowMind.Api.Models.Entities;
using FlowMind.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlowMind.Api.Controllers
{
    [ApiController]
    [Route("api/users/{UserId}/indice")]
    [Produces("application/json")]
    public class IndiceEquilibrioController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IIAService _iaService;
        private readonly IUrlHelper _urlHelper;

        public IndiceEquilibrioController(AppDbContext context, IIAService iaService, IUrlHelper urlHelper)
        {
            _context = context;
            _iaService = iaService;
            _urlHelper = urlHelper;
        }

        [HttpGet(Name = "GetIndiceHoje")]
        public async Task<ActionResult<object>> GetIndiceHoje(int UserId)
        {
            var indice = await CalcularIndiceAsync(UserId, DateTime.Today);
            var response = new
            {
                indice.Valor,
                indice.ResumoTexto,
                data = indice.Data.ToString("yyyy-MM-dd"),
                links = new[]
                {
                    new LinkDto { Href = _urlHelper.Link("GetIndiceHoje", new { UserId })!, Rel = "self", Method = "GET" },
                    new LinkDto { Href = _urlHelper.Link("GetIndiceSemanal", new { UserId })!, Rel = "semanal", Method = "GET" }
                }
            };

            return Ok(response);
        }

        [HttpGet("semanal", Name = "GetIndiceSemanal")]
        public async Task<ActionResult<object>> GetIndiceSemanal(int UserId, [FromQuery] int page = 1, [FromQuery] int pageSize = 7)
        {
            var semana = Enumerable.Range(0, 7)
                .Select(i => DateTime.Today.AddDays(-i))
                .Reverse()
                .ToList();

            var indices = new List<object>();
            foreach (var dia in semana)
            {
                var indice = await CalcularIndiceAsync(UserId, dia);
                indices.Add(new
                {
                    data = dia.ToString("yyyy-MM-dd"),
                    valor = indice.Valor,
                    resumo = indice.ResumoTexto
                });
            }

            var response = PaginationHelper.CreatePaginatedResponse(
                indices, page, pageSize, indices.Count, _urlHelper, "GetIndiceSemanal");

            return Ok(response);
        }

        private async Task<IndiceEquilibrio> CalcularIndiceAsync(int UserId, DateTime data)
        {
            var checkin = await _context.CheckinsDiarios
                .FirstOrDefaultAsync(c => c.UserId == UserId && c.Data.Date == data.Date);

            var eventos = await _context.EventosDias
                .Where(e => e.UserId == UserId && e.Data.Date == data.Date)
                .ToListAsync();

            var rotina = await _context.RotinasDiarias
                .FirstOrDefaultAsync(r => r.UserId == UserId && r.Data.Date == data.Date);

            double energia = checkin != null
                ? checkin.Energia switch
                {
                    Energia.Alta => 1.0,
                    Energia.Media => 0.6,
                    _ => 0.3
                }
                : 0.5;

            double pausas = Math.Min(eventos.Count(e => e.TipoEvento == TipoEvento.Pausa) * 0.2, 1.0);
            double consistencia = rotina != null ? 1.0 : 0.0;

            int valor = (int)Math.Round((energia * 0.4 + pausas * 0.3 + consistencia * 0.3) * 100);
            valor = Math.Clamp(valor, 0, 100);

            var prompt = $"Energia: {energia:P0}, {eventos.Count(e => e.TipoEvento == TipoEvento.Pausa)} pausas, rotina: {(rotina != null ? "sim" : "não")}. Dê um feedback positivo curto.";
            var resumo = await _iaService.GerarFeedbackAsync(prompt);

            var indice = new IndiceEquilibrio
            {
                UserId = UserId,
                Data = data,
                Valor = valor,
                ResumoTexto = resumo
            };

            var existente = await _context.IndiceEquilibrio
                .FirstOrDefaultAsync(i => i.UserId == UserId && i.Data.Date == data.Date);

            if (existente != null)
            {
                existente.Valor = valor;
                existente.ResumoTexto = resumo;
            }
            else
            {
                _context.IndiceEquilibrio.Add(indice);
            }

            await _context.SaveChangesAsync();
            return indice;
        }
    }
}