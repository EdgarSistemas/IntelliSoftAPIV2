using IntelliSoftAPIV2.Dtos.Proveedores;

namespace IntelliSoftAPIV2.Dtos.Compra
{
    public class CompraDetalleDto
    {
        public int IdCompra { get; set; }
        public string? ClaveCompra { get; set; }
        public DateTime? FechaCompra { get; set; }
        public string? Observacion { get; set; }
        public int? Estatus { get; set; }

        public ProveedorResponseDto Proveedor { get; set; } = new();
        public List<CompraDetalleItemDto> Detalles { get; set; } = new();
    }
}
