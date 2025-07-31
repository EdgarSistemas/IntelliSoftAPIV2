using IntelliSoftAPIV2.Dtos.Unidades;

namespace IntelliSoftAPIV2.Dtos.Productos
{
    public class ProductoDetalleDto
    {
        public int IdProductos { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public decimal PrecioActual { get; set; }
        public decimal? HectareaBase { get; set; }
        public List<ProductoInsumoDetalleDto> Insumos { get; set; } = new();
    }
}
