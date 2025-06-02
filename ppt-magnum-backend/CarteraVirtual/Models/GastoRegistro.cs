using System.ComponentModel.DataAnnotations.Schema;

namespace pdt.Models
{
    [Table("pdt_gastos_registro")]
    public class GastoRegistro
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int FondoId { get; set; }
        public string? Observaciones { get; set; }
        public string NombreComercio { get; set; } = string.Empty;
        public string TipoDocumento { get; set; } = string.Empty;

        // Nueva propiedad para propietario
        [Column("propietario")]
        public string? Propietario { get; set; }

        public FondoMonetario? Fondo { get; set; }
        public ICollection<GastoDetalle>? Detalles { get; set; }
    }
}
