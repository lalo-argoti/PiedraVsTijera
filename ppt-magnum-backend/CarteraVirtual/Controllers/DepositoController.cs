using Microsoft.EntityFrameworkCore;    
using Microsoft.AspNetCore.Mvc;
using pdt.Models;
using pdt.Data;

namespace pdt.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepositoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DepositoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("todos")]
        public IActionResult GetAll()
        {
            int propietarioId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");

            var depositos = _context.Depositos
                .Where(d => d.Propietario == propietarioId)
                .Select(d => new
                {
                    d.Id,
                    d.Fecha,
                    d.Remitente,
                    d.Propietario,
                    Detalles = d.Detalles.Select(det => new
                    {
                        det.Id,
                        det.FondoId,
                        det.MontoCOP,
                        det.MontoUSD,
                        det.ReferenciaPago
                    }).ToList()
                })
                .ToList();

            return Ok(depositos);
        }


        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var deposito = _context.Depositos
                .Include(d => d.Detalles)
                .FirstOrDefault(d => d.Id == id);

            if (deposito == null)
                return NotFound();

            return Ok(new
            {
                Encabezado = new
                {
                    deposito.Id,
                    deposito.Fecha,
                    deposito.Remitente,
                    deposito.Propietario
                },
                Detalles = deposito.Detalles.Select(d => new
                {
                    d.Id,
                    d.FondoId,
                    d.MontoCOP,
                    d.MontoUSD,
                    d.ReferenciaPago
                })
            });
        }
 
[HttpGet]
public IActionResult GetPorMes([FromQuery] int mes, [FromQuery] int anio)
{
    int propietario=1;// = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");

    var depositos = _context.Depositos
        .Include(d => d.Detalles)
        .Where(d => d.Fecha.Month == mes && d.Fecha.Year == anio && d.Propietario == propietario)
        .ToList();

    if (!depositos.Any())
        return Ok(new object[] { });


    var resultado = depositos.Select(deposito => new
    {
        Encabezado = new
        {
            deposito.Id,
            deposito.Fecha,
            deposito.Remitente,
            deposito.Propietario
        },
        Detalles = deposito.Detalles.Select(d => new
        {
            d.Id,
            d.FondoId,
            d.MontoCOP,
            d.MontoUSD,
            d.ReferenciaPago
        })
    });

    return Ok(resultado);
}

 
   
        [HttpPost]
        public IActionResult Create([FromBody] DepositoTransaccionDto transaccion)
        {
            if (transaccion == null || transaccion.Detalles == null || !transaccion.Detalles.Any())
                return BadRequest("Debe proporcionar encabezado y al menos un detalle.");

            int propietario = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");

            var deposito = new Deposito
            {
                Fecha = transaccion.Encabezado.Fecha,
                Remitente = transaccion.Encabezado.Remitente,
                Propietario = propietario,
                Detalles = transaccion.Detalles.Select(d => new DepositoDetalle
                {
                    FondoId = d.FondoId,
                    MontoCOP = d.MontoCOP,
                    MontoUSD = d.MontoUSD,
                    ReferenciaPago = d.ReferenciaPago
                }).ToList()
            };

            _context.Depositos.Add(deposito);

            foreach (var detalle in deposito.Detalles)
            {
                var fondo = _context.FondosMonetarios.FirstOrDefault(f => f.Id == detalle.FondoId);
                if (fondo != null)
                {
                    fondo.CapitalCOP += detalle.MontoCOP;
                    fondo.CapitalUSD += detalle.MontoUSD;
                }
            }

            _context.SaveChanges();
            return Ok(new { message = "Depósito registrado correctamente." });
        }


        // PUT: api/DepositoController/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] object model)
        {
            // TODO: Implementar lógica para actualizar un registro
            return NoContent();
        }

        // DELETE: api/DepositoController/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // TODO: Implementar lógica para eliminar un registro
            return NoContent();
        }
    }
}
