using System.ComponentModel.DataAnnotations.Schema;

namespace pdt.Models
{
    [Table("pdt_depositos")]
    public class Deposito
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string? Remitente { get; set; }
        public int Propietario { get; set; } 

        public List<DepositoDetalle> Detalles { get; set; } = new();
    }


}
