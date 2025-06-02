namespace pdt.Models
{
    public class UserGroup
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Permisos { get; set; }

        // Relación inversa (opcional)
        public ICollection<User>? Users { get; set; }
    }
}
