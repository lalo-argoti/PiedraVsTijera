using Microsoft.AspNetCore.Mvc;
using pdt.Data;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;  

using pdt.Models.DTO;
using pdt.Models;




namespace pdt.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class GastoRegistroController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GastoRegistroController(AppDbContext context)
        {
            _context = context;
        }
//----------------------------------------------------------        
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var detalles = _context.GastosDetalles
                    .Where(d => d.GastoRegistroId == id)
                    .ToList();

                if (!detalles.Any())
                    return NotFound(new { mensaje = "No se encontraron detalles para ese registro." });

                return Ok(detalles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener los datos", error = ex.Message });
            }
        }
//----------------------------------------------------------        

 [HttpGet]
public IActionResult GetByMesAnio([FromQuery] int mes, [FromQuery] int anio)
{
    try
    {
        int propietario = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");

var gastos = _context.GastosRegistros
    .Where(g => g.Propietario == propietario && g.Fecha.Month == mes && g.Fecha.Year == anio)
    .Include(g => g.Detalles) // Solo los detalles necesarios, sin .ThenInclude
    .ToList();

var resultado = gastos.Select(g => new GastoResumenMensualDto
{   Id=g.Id   ,
    Fecha = g.Fecha,
    FondoId = g.FondoId,
    Observaciones = g.Observaciones ?? string.Empty,
    Detalles = g.Detalles.Select(d => new GastoDetalle2Dto
    {
        Id = d.Id,
        GastoRegistroId = d.GastoRegistroId,
        GastoTipoId = d.GastoTipoId,
        MontoCOP = d.MontoCOP,
        MontoUSD = d.MontoUSD
    }).ToList()
}).ToList();

return Ok(resultado);

    }
    catch (Exception ex)
    {
        return BadRequest(new { mensaje = "Error al obtener registros", error = ex.Message });
    }
}

 
 
 
         [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var gasto = _context.GastosRegistros
                            .Include(g => g.Detalles)
                            .FirstOrDefault(g => g.Id == id);

            if (gasto == null)
                return NotFound();

            // Si tienes cascade delete configurado (como en OnModelCreating), esta línea es opcional
            _context.GastosDetalles.RemoveRange(gasto.Detalles);

            _context.GastosRegistros.Remove(gasto);
            _context.SaveChanges();

            return NoContent();
        }
//----------------------------------------------------------
          
        [HttpPost]
        public IActionResult Create([FromBody] GastoRegistroDto model)
        {

            try
            {
                int propietario = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");

                var registro = new GastoRegistro
                {
                    Propietario = propietario,
                    Fecha = model.Fecha,
                    Observaciones = model.Observaciones,
                    FondoId = model.FondoId,
                    Criterio = "default"
                };

                _context.GastosRegistros.Add(registro);
                _context.SaveChanges();

                foreach (var detalleDto in model.Detalles)
                {
                    var detalle = new GastoDetalle
                    {
                        GastoRegistroId = registro.Id,
                        GastoTipoId = detalleDto.GastoTipoId,
                        MontoCOP = detalleDto.MontoCOP,
                        MontoUSD = detalleDto.MontoUSD
                    };

                    _context.GastosDetalles.Add(detalle);
                }

                _context.SaveChanges();

                var responseDto = new GastoRegistroDto
                {
                    Fecha = registro.Fecha,
                    FondoId = registro.FondoId,
                    Observaciones = registro.Observaciones,
                    Detalles = model.Detalles
                };

                return CreatedAtAction(nameof(Create), new { id = registro.Id }, responseDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Error al guardar", error = ex.Message });
            }
        }

        
        [HttpPut("{id}")]
public IActionResult Update(int id, [FromBody] GastoRegistroDto model)
{
    try
    {
        int propietario = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");

        var registro = _context.GastosRegistros
            .Include(r => r.Detalles)
            .FirstOrDefault(r => r.Id == id && r.Propietario == propietario);

        if (registro == null)
        {
            return NotFound(new { mensaje = "Registro no encontrado" });
        }

        // Actualizar datos principales
        registro.Fecha = model.Fecha;
        registro.Observaciones = model.Observaciones;
        registro.FondoId = model.FondoId;
        registro.Criterio = "manual";

        // Eliminar detalles anteriores
        _context.GastosDetalles.RemoveRange(registro.Detalles);

        // Agregar nuevos detalles
        foreach (var detalleDto in model.Detalles)
        {
            var detalle = new GastoDetalle
            {
                GastoRegistroId = registro.Id,
                GastoTipoId = detalleDto.GastoTipoId,
                MontoCOP = detalleDto.MontoCOP,
                MontoUSD = detalleDto.MontoUSD
            };
            _context.GastosDetalles.Add(detalle);
        }

        _context.SaveChanges();

        var responseDto = new GastoRegistroDto
        {
            Fecha = registro.Fecha,
            FondoId = registro.FondoId,
            Observaciones = registro.Observaciones,
            Detalles = model.Detalles
        };

        return Ok(responseDto);
    }
    catch (Exception ex)
    {
        return BadRequest(new { mensaje = "Error al actualizar", error = ex.Message });
    }
}
        
        
        private int GenerateCustomId(int userId, DateTime fecha)
        {
            string baseId = "1";
            string userIdStr = userId.ToString().PadLeft(2, '0');
            string fechaStr = fecha.ToString("yyMMdd");
            Random rnd = new Random();
            string randomStr = rnd.Next(10, 99).ToString();
            string idStr = baseId + userIdStr + fechaStr + randomStr;

            return int.Parse(idStr);
        }
    }
}
