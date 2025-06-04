public class GastoDetalleDto
{
    public int GastoTipoId { get; set; }
    public decimal MontoCOP { get; set; }
    public decimal MontoUSD { get; set; }
}

public class GastoRegistroDto
{
    public DateTime Fecha { get; set; }
    public int FondoId { get; set; }
    public string Observaciones { get; set; } = string.Empty;
    public List<GastoDetalleDto> Detalles { get; set; } = new();
}
