namespace pdt.Models.DTO
{
    public class GastoResumenMensualDto
    {
        public DateTime Fecha { get; set; }

        public int FondoId { get; set; }

        public string Observaciones { get; set; } = string.Empty;

        public List<GastoDetalle2Dto> Detalles { get; set; } = new();
    }

    public class GastoDetalle2Dto
    {
        public int Id { get; set; }

        public int GastoRegistroId { get; set; }

        public int GastoTipoId { get; set; }

        public decimal MontoCOP { get; set; }

        public decimal MontoUSD { get; set; }
    }
}
