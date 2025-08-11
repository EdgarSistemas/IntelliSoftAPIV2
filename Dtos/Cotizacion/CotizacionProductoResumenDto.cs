namespace IntelliSoftAPIV2.Dtos.Cotizacion
{
    public class CotizacionProductoResumenDto
    {
        public int IdCotizacion { get; set; }
        public string? ClaveCotizacion { get; set; }
        public DateTime? FechaSolicitud { get; set; }
        public int Estatus { get; set; }

        public int CotizacionProductoId { get; set; }
        public int ProductoId { get; set; }
        public string? NombreProducto { get; set; }
        public decimal Hectareas { get; set; }
        public string? NombreCliente { get; set; }

        public decimal PrecioBase { get; set; }
        public decimal Ganancia { get; set; }
        public decimal PrecioConGanancia { get; set; }
        public decimal PrecioConRiesgo { get; set; }
        public decimal Total { get; set; }

        public decimal PorcentajeGanancia { get; set; }
        public decimal PorcentajeRiesgo { get; set; }
        public decimal AplicaRiesgo { get; set; }
    }
}
