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

        [Column("TipoGasto")]
        public int GastoTipoId { get; set; }

        public GastoTipo? TipoGasto { get; set; }

        public decimal Monto { get; set; }
    }
}
