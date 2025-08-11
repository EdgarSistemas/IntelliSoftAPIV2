namespace IntelliSoftAPIV2.Dtos.Cotizacion
{
    public class CotizacionPartidaDto
    {
        public int CotizacionProductoId { get; set; }
        public int ProductoId { get; set; }
        public string? NombreProducto { get; set; }

        public decimal Hectareas { get; set; }
        public decimal PorcentajeGanancia { get; set; }
        public decimal PorcentajeRiesgo { get; set; }
        public int AplicaRiesgo { get; set; }

        public List<CotizacionProductoDetalleDto> Detalles { get; set; } = new();

        // cálculos
        public decimal PrecioBase { get; set; }
        public decimal Ganancia { get; set; }
        public decimal PrecioConGanancia { get; set; }
        public decimal PrecioConRiesgo { get; set; }
        public decimal Total { get; set; }
    }
}
