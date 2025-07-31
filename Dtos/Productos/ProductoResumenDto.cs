namespace IntelliSoftAPIV2.Dtos.Productos
{
    public class ProductoResumenDto
    {
        public int IdProductos { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public decimal PrecioActual { get; set; }
        public decimal? HectareaBase { get; set; }
    }
}
