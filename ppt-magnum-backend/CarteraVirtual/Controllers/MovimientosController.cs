using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pdt.Data;

namespace pdt.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovimientosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MovimientosController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Endpoint protegido con JWT
        [Authorize]
        [HttpGet]
        public IActionResult GetAll()
        {
            var respuesta = new
            {
                mensaje = "Acceso autorizado a movimientos",
                fecha = DateTime.UtcNow
            };

            return Ok(respuesta);
        }

        // Resto de métodos sin cambios, puedes implementarlos luego si quieres
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            return Ok(new { mensaje = $"Consulta del movimiento {id}" });
        }

        [HttpPost]
        public IActionResult Create([FromBody] object model)
        {
            return CreatedAtAction(nameof(GetById), new { id = 0 }, model);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] object model)
        {
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return NoContent();
        }
    }
}
