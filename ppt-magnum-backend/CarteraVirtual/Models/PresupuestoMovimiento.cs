using System.ComponentModel.DataAnnotations.Schema;

namespace pdt.Models
{
    [Table("pdt_presupuestos")]
    public class PresupuestoMovimiento
    {
        public int Id { get; set; }

        [ForeignKey("GastoTipo")]
        public int GastoTipoId { get; set; }

        public int Mes { get; set; }

        public int Anio { get; set; }

        public decimal MontoPresupuestadoUSD { get; set; }

        public decimal MontoPresupuestadoCOP { get; set; }

        public GastoTipo? GastoTipo { get; set; }
    }
}
