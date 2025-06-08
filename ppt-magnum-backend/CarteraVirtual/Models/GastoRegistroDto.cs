using System.Text.Json.Serialization;

public class GastoDetalleDto
{
    public int? Id {get;set;}
    public int GastoTipoId { get; set; }
    public string GastoTipoNombre { get; set; } = string.Empty; // Agregado
    public decimal MontoCOP { get; set; }
    public decimal MontoUSD { get; set; }
    
}

public class GastoRegistroDto
{
    public DateTime Fecha { get; set; }

    public int FondoId { get; set; }  

    public string FondoNombre { get; set; } = string.Empty;

    public string Observaciones { get; set; } = string.Empty;

    public List<GastoDetalleDto> Detalles { get; set; } = new();
}
