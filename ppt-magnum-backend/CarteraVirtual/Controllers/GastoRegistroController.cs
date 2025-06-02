using Microsoft.AspNetCore.Mvc;
using pdt.Models;
using pdt.Data;

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

        [HttpPost]
        public IActionResult Create([FromBody] GastoRegistroDto model)
        {
            try
            {
                var registro = new GastoRegistro
                {
                    Fecha = model.Fecha,
                    Observaciones = model.Observaciones,
                    Criterio = model.Criterio,
                    Propietario = model.Propietario,
                    FondoId = model.FondoId
                };

                _context.GastoRegistros.Add(registro);
                _context.SaveChanges();

                foreach (var detalle in model.Detalles)
                {
                    _context.GastoDetalles.Add(new GastoDetalle
                    {
                        GastoRegistroId = registro.Id,
                        GastoTipoId = detalle.GastoTipoId,
                        Monto = detalle.Monto
                    });
                }

                _context.SaveChanges();
                return CreatedAtAction(nameof(Create), new { id = registro.Id }, registro);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Error al guardar", error = ex.Message });
            }
        }
    }
}
