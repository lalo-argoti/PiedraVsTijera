using System.ComponentModel.DataAnnotations.Schema;

namespace pdt.Models
{
    [Table("pdt_presupuestos")]
    public class PresupuestoMovimiento
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int GastoTipoId { get; set; }
        public int Mes { get; set; }
        public decimal MontoPresupuestado { get; set; }

        public User? Usuario { get; set; }
        public GastoTipo? GastoTipo { get; set; }
    }
}
