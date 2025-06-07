public class GastoRegistroPlanoDto
{
    public int GastoRegistroId { get; set; }
    public DateTime Fecha { get; set; }
    public int FondoId { get; set; }
    public string? Observaciones { get; set; }
    public int? GastoDetalleId { get; set; }
    public int? GastoTipoId { get; set; }
    public decimal? MontoCOP { get; set; }
    public decimal? MontoUSD { get; set; }
}
