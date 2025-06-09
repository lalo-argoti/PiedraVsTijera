using Microsoft.EntityFrameworkCore;

public class MovimientosioDto
{
    public string Tipo { get; set; } // "deposito" o "gasto"
    public DateTime Fecha { get; set; }
    public decimal MontoCOP { get; set; }
    public decimal MontoUSD { get; set; }
    public string Descripcion { get; set; } // opcional
}
