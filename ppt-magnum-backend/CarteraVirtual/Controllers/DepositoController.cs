
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

        // GET: api/DepositoController
        [HttpGet]
        public IActionResult GetAll()
        {
            // TODO: Implementar lógica para obtener todos los registros
            return Ok();
        }

        // GET: api/DepositoController/5
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            // TODO: Implementar lógica para obtener un registro por id
            return Ok();
        }

        // POST: api/DepositoController
        [HttpPost]
        public IActionResult Create([FromBody] DepositoTransaccionDto transaccion)
        {
            if (transaccion.Detalles == null || transaccion.Detalles.Count == 0)
            {
                return BadRequest("Debe proporcionar al menos un detalle de depósito.");
            }

            foreach (var detalle in transaccion.Detalles)
            {
                if (detalle.MontoCOP > 0)
                {
                    var depositoCop = new Deposito
                    {
                        Fecha = transaccion.Encabezado.Fecha,
                        FondoId = detalle.FondoId,
                        Monto = detalle.MontoCOP
                    };
                    _context.Depositos.Add(depositoCop);

                    // Actualiza el capital en COP
                    var fondo = _context.FondosMonetarios.FirstOrDefault(f => f.Id == detalle.FondoId);
                    if (fondo != null)
                    {
                        fondo.CapitalCOP += detalle.MontoCOP;
                    }
                }

                if (detalle.MontoUSD > 0)
                {
                    var depositoUsd = new Deposito
                    {
                        Fecha = transaccion.Encabezado.Fecha,
                        FondoId = detalle.FondoId,
                        Monto = detalle.MontoUSD
                    };
                    _context.Depositos.Add(depositoUsd);

                    // Actualiza el capital en USD
                    var fondo = _context.FondosMonetarios.FirstOrDefault(f => f.Id == detalle.FondoId);
                    if (fondo != null)
                    {
                        fondo.CapitalUSD += detalle.MontoUSD;
                    }
                }
            }

            _context.SaveChanges();
            return Ok(new { message = "Depósito registrado exitosamente" });
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
