using IntelliSoftAPIV2.Dtos.Unidades;

namespace IntelliSoftAPIV2.Dtos.Productos
{
    public class ProductoInsumoDetalleDto
    {
        public int InsumoId { get; set; }
        public string Nombre { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioPromedio { get; set; }
        public UnidadDto Unidad { get; set; }
    }
}
