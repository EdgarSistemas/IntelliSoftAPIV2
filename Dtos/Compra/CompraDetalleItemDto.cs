using IntelliSoftAPIV2.Dtos.Unidades;

namespace IntelliSoftAPIV2.Dtos.Compra
{
    public class CompraDetalleItemDto
    {
        public int IdCompraDetalle { get; set; }
        public int InsumoId { get; set; }
        public string InsumoNombre { get; set; } = string.Empty;
        public string Presentacion { get; set; } = string.Empty;
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }

        public UnidadDto Unidad { get; set; } = new();
    }
}
