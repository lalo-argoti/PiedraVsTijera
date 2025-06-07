using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Mvc;
using pdt.Models;
using pdt.Data;

namespace pdt.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GastoDetalleController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GastoDetalleController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/GastoDetalleController
        [HttpGet]
        public IActionResult GetAll()
        {
            // TODO: Implementar lógica para obtener todos los registros
            return Ok();
        }
        
        
   [HttpGet("por-fecha")]
public IActionResult GetByMesAnio([FromQuery] int mes, [FromQuery] int anio)
{
    try
    {
        int propietario = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");

var registros = _context.GastosRegistros
    .Where(g => g.Propietario == propietario && 
                g.Fecha.Month == mes && 
                g.Fecha.Year == anio)
    .Include(g => g.Detalles)  // <- aquí cargas los detalles
    .Select(g => new GastoRegistroDto
    {
        Fecha = g.Fecha,
        FondoId = g.FondoId,
        Observaciones = g.Observaciones,
        Detalles = g.Detalles.Select(d => new GastoDetalleDto
        {
            GastoTipoId = d.GastoTipoId,
            MontoCOP = d.MontoCOP,
            MontoUSD = d.MontoUSD
        }).ToList()
    })
    .ToList();

        return Ok(registros);
    }
    catch (Exception ex)
    {
        return BadRequest(new { mensaje = "Error al obtener registros", error = ex.Message });
    }
}
        
        // GET: api/GastoDetalleController/5
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            // TODO: Implementar lógica para obtener un registro por id
            return Ok();
        }

        // POST: api/GastoDetalleController
        [HttpPost]
        public IActionResult Create([FromBody] object model)
        {
            // TODO: Implementar lógica para crear un registro
            return CreatedAtAction(nameof(GetById), new { id = 0 }, model);
        }

        // PUT: api/GastoDetalleController/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] object model)
        {
            // TODO: Implementar lógica para actualizar un registro
            return NoContent();
        }

        // DELETE: api/GastoDetalleController/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // TODO: Implementar lógica para eliminar un registro
            return NoContent();
        }
    }
}
