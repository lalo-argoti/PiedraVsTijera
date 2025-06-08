namespace pdt.Models
{
    // DTO para el encabezado de depósitos
    public class DepositoEncabezadoDto
    {
        public int Id { get; set; }  // Para operaciones de update/delete si se usan
        public DateTime Fecha { get; set; }
        public string? Remitente { get; set; }
    }

    // DTO para el detalle de depósitos
    public class DepositoDetalleDto
    {
        public int Id { get; set; }  // Para update/delete
        public int DepositoId { get; set; }  // FK al encabezado
        public int FondoId { get; set; }
        public decimal MontoCOP { get; set; }
        public decimal MontoUSD { get; set; }
        public string ReferenciaPago { get; set; } = string.Empty;
    }

    // DTO para enviar toda la transacción
    public class DepositoTransaccionDto
    {
        public DepositoEncabezadoDto Encabezado { get; set; } = new();
        public List<DepositoDetalleDto> Detalles { get; set; } = new();
    }
}
