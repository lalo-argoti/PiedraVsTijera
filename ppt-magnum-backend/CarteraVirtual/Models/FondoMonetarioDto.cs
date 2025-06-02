namespace pdt.Models
{
    public class FondoMonetarioDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Tipo { get; set; } = "Caja";
        public decimal CapitalCOP { get; set; }
        public decimal CapitalUSD { get; set; }
    }
}
