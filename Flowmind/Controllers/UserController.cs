// Controllers/v1/UsersController.cs
using Flowmind.Models;
using FlowMind.Api.Data;
using FlowMind.Api.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlowMind.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/users")]
    [ApiVersion("1.0")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetUsers(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return BadRequest(new { message = "Parâmetros de paginação inválidos." });

            var totalItems = await _context.Users.CountAsync();
            var users = await _context.Users
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email
                    // Adicione os campos que quiser exibir
                })
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var response = new
            {
                pageNumber,
                pageSize,
                totalItems,
                totalPages,
                items = users
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetUser(int id)
        {
            var user = await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound(new { message = "Usuário não encontrado." });

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<object>> PostUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetUser),
                new { id = user.Id },
                new { user.Id, user.Name, user.Email }
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, [FromBody] User user)
        {
            if (id != user.Id)
                return BadRequest(new { message = "ID na URL diferente do corpo." });

            var existing = await _context.Users.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = "Usuário não encontrado." });

            existing.Name = user.Name;
            existing.Email = user.Email;
            // Atualize outros campos

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(new { message = "Usuário não encontrado." });

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}