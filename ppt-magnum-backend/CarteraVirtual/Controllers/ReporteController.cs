
using Microsoft.AspNetCore.Mvc;
using pdt.Models;
using pdt.Data;

namespace pdt.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReporteController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReporteController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/ReporteController
        [HttpGet]
        public IActionResult GetAll()
        {
            // TODO: Implementar lógica para obtener todos los registros
            return Ok();
        }

        // GET: api/ReporteController/5
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            // TODO: Implementar lógica para obtener un registro por id
            return Ok();
        }

        // POST: api/ReporteController
        [HttpPost]
        public IActionResult Create([FromBody] object model)
        {
            // TODO: Implementar lógica para crear un registro
            return CreatedAtAction(nameof(GetById), new { id = 0 }, model);
        }

        // PUT: api/ReporteController/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] object model)
        {
            // TODO: Implementar lógica para actualizar un registro
            return NoContent();
        }

        // DELETE: api/ReporteController/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // TODO: Implementar lógica para eliminar un registro
            return NoContent();
        }
    }
}
