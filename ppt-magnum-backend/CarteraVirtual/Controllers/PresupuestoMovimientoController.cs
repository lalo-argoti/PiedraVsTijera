using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pdt.Models;
using pdt.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;


namespace pdt.Controllers
{
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
[HttpGet("ejecucion")]
public async Task<IActionResult> GetPresupuestoEjecucion([FromQuery] int mes, [FromQuery] int anio, [FromQuery] int tipoGastoId)
{
    int propietario = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");

    // Obtener presupuesto para ese tipo, mes y año
    var presupuesto = await _context.PresupuestoMovimientos
        .Where(p => p.GastoTipoId == tipoGastoId && p.Mes == mes && p.Anio == anio)
        .FirstOrDefaultAsync();

    // Obtener gastos (ejecución) relacionados a ese tipo y rango de fechas
    // Asumiendo que GastosDetalles tienen la propiedad GastoTipoId (si no, habrá que incluir y filtrar)
    var gastosEjecutados = await _context.GastosDetalles
        .Include(d => d.GastoRegistro)
        .Where(d => d.GastoRegistro.Fecha.Month == mes
                    && d.GastoRegistro.Fecha.Year == anio
                    && d.GastoTipoId == tipoGastoId)
        .ToListAsync();

    decimal montoEjecutadoCOP = gastosEjecutados.Sum(g => g.MontoCOP);
    decimal montoEjecutadoUSD = gastosEjecutados.Sum(g => g.MontoUSD);

    return Ok(new
    {
        Presupuestado = new
        {
            MontoCOP = presupuesto?.MontoPresupuestadoCOP ?? 0,
            MontoUSD = presupuesto?.MontoPresupuestadoUSD ?? 0
        },
        Ejecutado = new
        {
            MontoCOP = montoEjecutadoCOP,
            MontoUSD = montoEjecutadoUSD
        }
    });
}

        
    [HttpGet("balance")]
    public async Task<IActionResult> GetBalanceMovimientos([FromQuery] DateTime fecha_inicio, [FromQuery] DateTime fecha_fin)
    {
        int propietario= int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");

var depositos = await _context.Depositos
    .Where(d => d.Fecha >= fecha_inicio && d.Fecha <= fecha_fin)
    .Select(d => new MovimientosioDto
    {
        Tipo = "deposito",
        Fecha = d.Fecha,
        MontoCOP = d.Detalles.Sum(det => det.MontoCOP),
        MontoUSD = d.Detalles.Sum(det => det.MontoUSD),
        Descripcion = d.Remitente ?? string.Empty
    })
    .ToListAsync();

var gastos = await _context.GastosDetalles
    .Include(d => d.GastoRegistro)
    .Where(d => d.GastoRegistro!.Fecha >= fecha_inicio && d.GastoRegistro.Fecha <= fecha_fin)
    .Select(d => new MovimientosioDto
    {
        Tipo = "gasto",
        Fecha = d.GastoRegistro!.Fecha,
        MontoCOP = d.MontoCOP,
        MontoUSD = d.MontoUSD,
        Descripcion = d.GastoRegistro!.Observaciones ?? string.Empty
    })
    .ToListAsync();

        var movimientos = depositos.Concat(gastos)
                                   .OrderBy(m => m.Fecha)
                                   .ToList();

        return Ok(movimientos);
    }
        
        // ✅ Este endpoint carga Fondos y Tipos de Gasto del usuario logueado
        [HttpGet("init")]
        public IActionResult GetInitData()
        {
            int userId =  1;//int.Parse(User.FindFirst("UserId")?.Value ?? "0");

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
