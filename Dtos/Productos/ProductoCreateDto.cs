namespace IntelliSoftAPIV2.Dtos.Productos
{
    public class ProductoCreateDto
    {
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public decimal? HectareaBase { get; set; }
        public decimal PorcentajeGanancia { get; set; }
        public decimal PorcentajeRiesgo { get; set; }
        public List<ProductoInsumoDto> Insumos { get; set; } = new();
    }
}
