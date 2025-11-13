using AutoMapper;
using FlowMind.Api.Data;
using FlowMind.Api.DTOs.Response;
using FlowMind.Api.Models.Entities;
using FlowMind.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlowMind.Api.Controllers
{
    [ApiController]
    [Route("api/users/{UserId}/rotinas")]
    [Produces("application/json")]
    public class RotinaController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IIAService _iaService;

        public RotinaController(
            AppDbContext context,
            IMapper mapper,
            IIAService iaService)
        {
            _context = context;
            _mapper = mapper;
            _iaService = iaService;
        }

        [HttpPost(Name = "GerarRotina")]
        public async Task<ActionResult<RotinaResponse>> GerarRotina(int UserId)
        {
            if (!await _context.Users.AnyAsync(u => u.Id == UserId))
                return NotFound(new { message = "Usuário não encontrado." });

            var hoje = DateTime.Today;
            if (await _context.RotinasDiarias.AnyAsync(r => r.UserId == UserId && r.Data.Date == hoje))
                return BadRequest(new { message = "Rotina já gerada hoje." });

            var checkin = await _context.CheckinsDiarios
                .FirstOrDefaultAsync(c => c.UserId == UserId && c.Data.Date == hoje);

            if (checkin == null)
                return BadRequest(new { message = "Faça o check-in diário primeiro." });

            var prompt = $$"""
                Crie uma agenda equilibrada de 8h para uma pessoa que está se sentindo {{checkin.Humor}}
                com energia {{checkin.Energia}} e sono {{checkin.Sono}}. Inclua blocos de foco, pausas e recompensa.
                Retorne APENAS JSON válido com: blocos, totalFoco, totalPausas.
                """;

            var rotinaJson = await _iaService.GerarRotinaAsync(prompt);

            var rotina = new RotinaDiaria
            {
                UserId = UserId,
                Data = hoje,
                RotinaJson = rotinaJson
            };

            _context.RotinasDiarias.Add(rotina);
            await _context.SaveChangesAsync();

            var dto = _mapper.Map<RotinaResponse>(rotina);
            return CreatedAtRoute("GetRotinaHoje", new { UserId }, dto);
        }

        [HttpGet("hoje", Name = "GetRotinaHoje")]
        public async Task<ActionResult<RotinaResponse>> GetRotinaHoje(int UserId)
        {
            var rotina = await _context.RotinasDiarias
                .FirstOrDefaultAsync(r => r.UserId == UserId && r.Data.Date == DateTime.Today);

            if (rotina == null)
                return NotFound(new { message = "Rotina não encontrada para hoje." });

            return Ok(_mapper.Map<RotinaResponse>(rotina));
        }

        // ... outros métodos sem Links
    }
}