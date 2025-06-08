using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using pdt.Models;

[Table("pdt_depositos_detalle")]
public class DepositoDetalle
{
    [Key]
        
    public int Id { get; set; }

    public int DepositoId { get; set; }
    [ForeignKey("DepositoId")]
    public Deposito? Deposito { get; set; }

    public int FondoId { get; set; }
    public decimal MontoCOP { get; set; }
    public decimal MontoUSD { get; set; }

    public string ReferenciaPago { get; set; } = string.Empty;
}
