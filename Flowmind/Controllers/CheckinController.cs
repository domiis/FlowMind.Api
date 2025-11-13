// Controllers/CheckinController.cs
using AutoMapper;
using FlowMind.Api.Data;
using FlowMind.Api.DTOs.Request;
using FlowMind.Api.DTOs.Response;
using FlowMind.Api.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlowMind.Api.Controllers
{
    [ApiController]
    [Route("api/users/{idUsuario}/checkins")]
    [Produces("application/json")]
    public class CheckinController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CheckinController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet(Name = "GetCheckins")]
        public async Task<ActionResult<object>> GetCheckins(
            int idUsuario,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (!await _context.Users.AnyAsync(u => u.Id == idUsuario))
                return NotFound(new { message = "Usuário não encontrado." });

            var query = _context.CheckinsDiarios
                .Where(c => c.UserId == idUsuario)
                .OrderByDescending(c => c.Data);

            var total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtos = _mapper.Map<List<CheckinResponse>>(items);
            var totalPages = (int)Math.Ceiling(total / (double)pageSize);

            return Ok(new
            {
                page,
                pageSize,
                total,
                totalPages,
                items = dtos
            });
        }

        [HttpGet("{id}", Name = "GetCheckin")]
        public async Task<ActionResult<CheckinResponse>> GetCheckin(int idUsuario, int id)
        {
            var checkin = await _context.CheckinsDiarios
                .FirstOrDefaultAsync(c => c.UserId == idUsuario && c.Id == id);

            if (checkin == null)
                return NotFound(new { message = "Check-in não encontrado." });

            return Ok(_mapper.Map<CheckinResponse>(checkin));
        }

        [HttpPost(Name = "PostCheckin")]
        public async Task<ActionResult<CheckinResponse>> PostCheckin(int idUsuario, CheckinCreateRequest request)
        {
            var usuario = await _context.Users.FindAsync(idUsuario);
            if (usuario == null)
                return NotFound(new { message = "Usuário não encontrado." });

            if (await _context.CheckinsDiarios.AnyAsync(c => c.UserId == idUsuario && c.Data.Date == DateTime.Today))
                return BadRequest(new { message = "Check-in já realizado hoje." });

            var checkin = _mapper.Map<CheckinDiario>(request);
            checkin.UserId = idUsuario;
            checkin.Data = DateTime.Today;

            _context.CheckinsDiarios.Add(checkin);
            await _context.SaveChangesAsync();

            var dto = _mapper.Map<CheckinResponse>(checkin);
            return CreatedAtRoute("GetCheckin", new { idUsuario, id = checkin.Id }, dto);
        }

        [HttpPut("{id}", Name = "PutCheckin")]
        public async Task<IActionResult> PutCheckin(int idUsuario, int id, CheckinCreateRequest request)
        {
            var checkin = await _context.CheckinsDiarios
                .FirstOrDefaultAsync(c => c.UserId == idUsuario && c.Id == id);

            if (checkin == null)
                return NotFound(new { message = "Check-in não encontrado." });

            checkin.Humor = request.Humor;
            checkin.Energia = request.Energia;
            checkin.Sono = request.Sono;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteCheckin")]
        public async Task<IActionResult> DeleteCheckin(int idUsuario, int id)
        {
            var checkin = await _context.CheckinsDiarios
                .FirstOrDefaultAsync(c => c.UserId == idUsuario && c.Id == id);

            if (checkin == null)
                return NotFound(new { message = "Check-in não encontrado." });

            _context.CheckinsDiarios.Remove(checkin);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}