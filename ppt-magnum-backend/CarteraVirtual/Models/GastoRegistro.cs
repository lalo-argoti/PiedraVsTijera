using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;


namespace pdt.Models
{
    [Table("pdt_gastos_registro")]
    public class GastoRegistro
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }

        [Column("FondoMonetario")]
        public int FondoId { get; set; }

        public string? Observaciones { get; set; }

        public string Criterio { get; set; } = string.Empty;

        [Column("propietario")]
        public int Propietario { get; set; }

        public FondoMonetario? Fondo { get; set; }

        [InverseProperty("GastoRegistro")]  // ← Esta es la adición segura

        
        public ICollection<GastoDetalle> Detalles { get; set; } = new List<GastoDetalle>();
    }
}
