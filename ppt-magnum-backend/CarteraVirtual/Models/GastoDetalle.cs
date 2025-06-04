using System.ComponentModel.DataAnnotations.Schema;

namespace pdt.Models
{
    [Table("pdt_gasto_detalle")]
    public class GastoDetalle
    {
        public int Id { get; set; }

        public int GastoRegistroId { get; set; }

        [ForeignKey("GastoRegistroId")]
        public GastoRegistro? GastoRegistro { get; set; }

        [Column("GastoTipoId")]
        public int GastoTipoId { get; set; }

        public GastoTipo? TipoGasto { get; set; }

        public decimal MontoCOP { get; set; }

        public decimal MontoUSD { get; set; }
    }
}
