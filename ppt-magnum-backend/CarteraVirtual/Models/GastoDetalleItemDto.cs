namespace CarteraVirtual.Dtos.GastoRegistro
{
    public class GastoDetalleItemDto
    {
        public int GastoTipoId { get; set; }

        public string GastoTipoNombre { get; set; } = string.Empty;

        public decimal MontoCOP { get; set; }

        public decimal MontoUSD { get; set; }
    }
}

/*
var detalleDto = new GastoDetalleItemDto
{
    GastoTipoId = detalle.GastoTipoId,
    GastoTipoNombre = detalle.TipoGasto?.Nombre ?? "Desconocido",
    MontoCOP = detalle.MontoCOP,
    MontoUSD = detalle.MontoUSD
};
*/
