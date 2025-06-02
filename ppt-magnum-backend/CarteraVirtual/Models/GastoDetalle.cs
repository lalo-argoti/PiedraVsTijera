using System.ComponentModel.DataAnnotations.Schema;

namespace pdt.Models
{
    [Table("pdt_gastos_detalle")]
    public class GastoDetalle
    {
        public int Id { get; set; }
        public int RegistroId { get; set; }
        public int GastoTipoId { get; set; }
        public decimal Monto { get; set; }

        public GastoRegistro? Registro { get; set; }
        public GastoTipo? GastoTipo { get; set; }
    }
}
