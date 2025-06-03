using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pdt.Models;
using pdt.Data;
using System.Linq;

namespace pdt.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PresupuestoMovimientoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PresupuestoMovimientoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(); // Puedes implementar si necesitas.
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            return Ok(); // Puedes implementar si necesitas.
        }

        [HttpPost]
        public IActionResult Create([FromBody] object model)
        {
            return CreatedAtAction(nameof(GetById), new { id = 0 }, model); // Ajustar según lógica real
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] object model)
        {
            return NoContent(); // Implementar lógica si es necesario
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return NoContent(); // Implementar lógica si es necesario
        }

        // ✅ Este endpoint carga Fondos y Tipos de Gasto del usuario logueado
        [HttpGet("init")]
        public IActionResult GetInitData()
        {
            int userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            var fondos = _context.FondosMonetarios
                .Where(f => f.Propietario == userId)
                .Select(f => new FondoMonetarioDto
                {
                    Id = f.Id,
                    Nombre = f.Nombre,
                    Tipo = f.Tipo,
                    CapitalCOP = f.CapitalCOP,
                    CapitalUSD = f.CapitalUSD
                })
                .ToList();

            var tiposGasto = _context.GastosTipos
                .Where(t => t.Propietario == userId)
                .Select(t => new GastoTipoDto
                {
                    Id = t.Id,
                    Nombre = t.Nombre,
                    Presupuestocol = t.Presupuestocol,
                    Presupuestousd = t.Presupuestousd
                })
                .ToList();

            return Ok(new
            {
                fondos,
                tiposGasto
            });
        }
    }
}
