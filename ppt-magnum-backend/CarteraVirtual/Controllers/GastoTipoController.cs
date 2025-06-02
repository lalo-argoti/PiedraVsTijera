using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pdt.Data;
using pdt.Models;
using System.Security.Claims;

namespace pdt.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class GastoTipoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GastoTipoController(AppDbContext context)
        {
            _context = context;
        }

        // 🧠 Método para obtener el ID del usuario desde el token
        private int GetUserId()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            int userId = GetUserId();

            var tipos = _context.GastosTipos
                .Where(t => t.Propietario == userId)
                // Proyectamos directamente a DTO para evitar incluir navegación
                .Select(t => new GastoTipoDto
                {
                    Id = t.Id,
                    Nombre = t.Nombre,
                    Presupuestocol = t.Presupuestocol,
                    Presupuestousd = t.Presupuestousd
                })
                .ToList();

            return Ok(tipos);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            int userId = GetUserId();

            var tipo = _context.GastosTipos
                .Where(t => t.Id == id && t.Propietario == userId)
                .Select(t => new GastoTipoDto
                {
                    Id = t.Id,
                    Nombre = t.Nombre,
                    Presupuestocol = t.Presupuestocol,
                    Presupuestousd = t.Presupuestousd
                })
                .FirstOrDefault();

            if (tipo == null)
                return NotFound();

            return Ok(tipo);
        }

        [HttpPost]
        public IActionResult Create([FromBody] GastoTipoDto model)
        {
            int userId = GetUserId();

            var entity = new GastoTipo
            {
                Nombre = model.Nombre,
                Presupuestocol = model.Presupuestocol,
                Presupuestousd = model.Presupuestousd,
                Propietario = userId
            };

            _context.GastosTipos.Add(entity);
            _context.SaveChanges();

            var dto = new GastoTipoDto
            {
                Id = entity.Id,
                Nombre = entity.Nombre,
                Presupuestocol = entity.Presupuestocol,
                Presupuestousd = entity.Presupuestousd
            };

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, dto);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] GastoTipoDto model)
        {
            int userId = GetUserId();

            var existente = _context.GastosTipos
                .FirstOrDefault(t => t.Id == id && t.Propietario == userId);

            if (existente == null)
                return NotFound();

            existente.Nombre = model.Nombre;
            existente.Presupuestocol = model.Presupuestocol;
            existente.Presupuestousd = model.Presupuestousd;

            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            int userId = GetUserId();

            var tipo = _context.GastosTipos
                .FirstOrDefault(t => t.Id == id && t.Propietario == userId);

            if (tipo == null)
                return NotFound();

            _context.GastosTipos.Remove(tipo);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
