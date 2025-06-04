namespace pdt.Models
{
    public class DepositoTransaccionDto
    {
        public DepositoEncabezadoDto Encabezado { get; set; } = new();
        public List<DepositoDetalleDto> Detalles { get; set; } = new();
    }

    public class DepositoEncabezadoDto
    {
        public DateTime Fecha { get; set; }
        public string? Descripcion { get; set; }
        public int Mes { get; set; }
        public int Anio { get; set; }
        public string? Referencia { get; set; }
    }

    public class DepositoDetalleDto
    {
        public int FondoId { get; set; }
        public decimal MontoCOP { get; set; }
        public decimal MontoUSD { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public string ReferenciaPago { get; set; } = string.Empty;
    }
}
