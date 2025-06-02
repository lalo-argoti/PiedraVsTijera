using System.ComponentModel.DataAnnotations.Schema;

namespace pdt.Models
{
    [Table("pdt_depositos")]
    public class Deposito
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int FondoId { get; set; }
        public decimal Monto { get; set; }

        public FondoMonetario? Fondo { get; set; }
    }
}

