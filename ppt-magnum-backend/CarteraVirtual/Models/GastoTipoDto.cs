namespace pdt.Models
{
    public class GastoTipoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Presupuestocol { get; set; }
        public decimal Presupuestousd { get; set; }
    }
}
