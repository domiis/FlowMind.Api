// Controllers/EventoDiaController.cs
using FlowMind.Api.Data;
using FlowMind.Api.DTOs.Links;
using FlowMind.Api.Helpers;
using FlowMind.Api.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlowMind.Api.Controllers
{
    [ApiController]
    [Route("api/users/{UserId}/eventos")]
    [Produces("application/json")]
    public class EventoDiaController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IUrlHelper _urlHelper;

        public EventoDiaController(AppDbContext context, IUrlHelper urlHelper)
        {
            _context = context;
            _urlHelper = urlHelper;
        }

        [HttpGet(Name = "GetEventos")]
        public async Task<ActionResult<object>> GetEventos(
            int UserId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (!await _context.Users.AnyAsync(u => u.Id == UserId))
                return NotFound(new { message = "Usuário não encontrado." });

            var query = _context.EventosDias
                .Where(e => e.UserId == UserId)
                .OrderByDescending(e => e.Data);

            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var response = PaginationHelper.CreatePaginatedResponse(
                items, page, pageSize, total, _urlHelper, "GetEventos");

            return Ok(response);
        }

        [HttpGet("{id}", Name = "GetEvento")]
        public async Task<ActionResult<object>> GetEvento(int UserId, int id)
        {
            var evento = await _context.EventosDias.FirstOrDefaultAsync(e => e.UserId == UserId && e.Id == id);
            if (evento == null)
                return NotFound(new { message = "Evento não encontrado." });

            var response = new
            {
                evento.Id,
                evento.Data,
                evento.TipoEvento,
                links = new[]
                {
                    new LinkDto { Href = _urlHelper.Link("GetEvento", new { UserId, id })!, Rel = "self", Method = "GET" }
                }
            };

            return Ok(response);
        }

        [HttpPost("crise", Name = "PostEventoCrise")]
        public async Task<IActionResult> RegistrarCrise(int UserId)
            => await RegistrarEvento(UserId, TipoEvento.Crise, "Respire fundo. Você está no controle.");

        [HttpPost("pausa", Name = "PostEventoPausa")]
        public async Task<IActionResult> RegistrarPausa(int UserId)
            => await RegistrarEvento(UserId, TipoEvento.Pausa, "Pausa registrada. Ótimo autocuidado!");

        [HttpPost("recompensa", Name = "PostEventoRecompensa")]
        public async Task<IActionResult> RegistrarRecompensa(int UserId)
            => await RegistrarEvento(UserId, TipoEvento.Recompensa, "Parabéns! Você merece essa recompensa.");

        private async Task<IActionResult> RegistrarEvento(int UserId, TipoEvento tipo, string mensagem)
        {
            if (!await _context.Users.AnyAsync(u => u.Id == UserId))
                return NotFound(new { message = "Usuário não encontrado." });

            var evento = new EventoDia
            {
                UserId = UserId,
                Data = DateTime.Now,
                TipoEvento = tipo
            };

            _context.EventosDias.Add(evento);
            await _context.SaveChangesAsync();

            var response = new
            {
                mensagem,
                tipo = tipo.ToString(),
                registradoEm = DateTime.Now,
                links = new[]
                {
                    new LinkDto { Href = _urlHelper.Link("GetIndiceHoje", new { UserId })!, Rel = "indice-hoje", Method = "GET" },
                    new LinkDto { Href = _urlHelper.Link("GetEvento", new { UserId, id = evento.Id })!, Rel = "self", Method = "GET" }
                }
            };

            return CreatedAtRoute("GetEvento", new { UserId, id = evento.Id }, response);
        }

        [HttpPut("{id}", Name = "PutEvento")]
        public async Task<IActionResult> UpdateEvento(int UserId, int id, EventoDia update)
        {
            var evento = await _context.EventosDias.FirstOrDefaultAsync(e => e.UserId == UserId && e.Id == id);
            if (evento == null)
                return NotFound(new { message = "Evento não encontrado." });

            evento.TipoEvento = update.TipoEvento;
            evento.Data = update.Data;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteEvento")]
        public async Task<IActionResult> DeleteEvento(int UserId, int id)
        {
            var evento = await _context.EventosDias.FirstOrDefaultAsync(e => e.UserId == UserId && e.Id == id);
            if (evento == null)
                return NotFound(new { message = "Evento não encontrado." });

            _context.EventosDias.Remove(evento);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}