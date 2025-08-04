namespace IntelliSoftAPIV2.Dtos.Cotizacion
{
    public class CotizacionDto
    {
        public int IdCotizacion { get; set; }
        public string ClaveCotizacion { get; set; }
        public int ProductoId { get; set; }
        public string? UsuarioId { get; set; }
        public decimal Hectareas { get; set; }
        public int EstadoSolicitud { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public decimal PrecioBase { get; set; }
        public decimal PrecioConGanancia { get; set; }
        public decimal PrecioConRiesgo { get; set; }

        public List<CotizacionDetalleDto> Detalles { get; set; }

        public decimal Total => Detalles?.Sum(d => d.Subtotal) ?? 0;
    }
}
