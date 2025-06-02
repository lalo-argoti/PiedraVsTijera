using System.ComponentModel.DataAnnotations.Schema;

namespace pdt.Models
{
    [Table("pdt_crtr_gastos_tipo")]
    public class GastoTipo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Propietario { get; set; }

        [Column("presupuesto_col")]
        public decimal Presupuestocol { get; set; }

        [Column("presupuesto_usd")]
        public decimal Presupuestousd { get; set; }

        public User? PropietarioNavigation { get; set; }
    }
}
