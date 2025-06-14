using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace pdt.Models
{
    [Table("pdt_fondos_monetarios")]
    public class FondoMonetario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Tipo { get; set; } = "Caja"; // "Caja" o "Bancaria"
        
        [Column("capitalCOP")]
        public decimal CapitalCOP { get; set; } = 0;
        
        [Column("capitalUSD")]
        public decimal CapitalUSD { get; set; } = 0;
        
        public int Propietario { get; set; }
        public User? PropietarioNavigation { get; set; }
    }
}
