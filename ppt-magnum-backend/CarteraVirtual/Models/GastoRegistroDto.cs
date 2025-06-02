using System;
using System.Collections.Generic;

namespace pdt.Models
{
    public class GastoRegistroDto
    {
        public DateTime Fecha { get; set; }
        public string? Observaciones { get; set; }
        public string Criterio { get; set; } = string.Empty;
        public string Propietario { get; set; } = string.Empty;
        public int FondoId { get; set; }
        public List<GastoDetalleDto> Detalles { get; set; } = new();
    }

    public class GastoDetalleDto
    {
        public int GastoTipoId { get; set; }
        public decimal Monto { get; set; }
    }
}
