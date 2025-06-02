using System.ComponentModel.DataAnnotations.Schema;

namespace pdt.Models
{
    [Table("pdt_users")]
    public class User
    
    {
        public int Id { get; set; }
        public int Grupo { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        // Relaciones
        public UserGroup? GrupoNavigation { get; set; } // Foreign Key hacia UserGroup
        public ICollection<GastoTipo>? GastosTipos { get; set; } // Inversa hacia gastos
        public ICollection<FondoMonetario>? FondosMonetarios { get; set; }

    }

}
