using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pdt.Data;
using pdt.Models;

namespace pdt.Controllers
{
    [ApiController]
    [Route("api/gastos")]
public class PresupuestoMovimientosConfigController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PresupuestoMovimientosConfigController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int mes, [FromQuery] int anio)
        {
            var movimientos = await _context.PresupuestoMovimientos
                .Where(p => p.Mes == mes && p.Anio == anio)
                .Include(p => p.GastoTipo)
                .Select(p => new {
                    p.Id,
                    p.GastoTipoId,
                    tipoGastoNombre = p.GastoTipo!.Nombre,
                    p.MontoPresupuestadoCOP,
                    p.MontoPresupuestadoUSD
                })
                .ToListAsync();

            return Ok(movimientos);
        }

[HttpPost]
public async Task<IActionResult> Create([FromBody] PresupuestoMovimiento model)
{
    try
    {
        _context.PresupuestoMovimientos.Add(model);
        await _context.SaveChangesAsync();
        return Ok(model);
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Error al guardar: {ex.Message}");
    }
}




        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _context.PresupuestoMovimientos.FindAsync(id);
            return item != null ? Ok(item) : NotFound();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PresupuestoMovimiento model)
        {
            if (id != model.Id) return BadRequest();

            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.PresupuestoMovimientos.FindAsync(id);
            if (item == null) return NotFound();

            _context.PresupuestoMovimientos.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("tipos-gasto")]
        public async Task<IActionResult> GetTiposGasto()
        {
            var tipos = await _context.GastosTipos
                .Select(g => new { g.Id, g.Nombre })
                .ToListAsync();

            return Ok(tipos);
        }

        [HttpGet("init")]
        public async Task<IActionResult> Init()
        {
            var tiposGasto = await _context.GastosTipos.Select(g => new { g.Id, g.Nombre }).ToListAsync();
            return Ok(new { tiposGasto });
        }
    }
}
