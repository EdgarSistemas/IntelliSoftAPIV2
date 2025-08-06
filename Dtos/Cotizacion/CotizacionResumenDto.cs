namespace IntelliSoftAPIV2.Dtos.Cotizacion
{
    public class CotizacionResumenDto
    {
        public int IdCotizacion { get; set; }
        public string ClaveCotizacion { get; set; }
        public string NombreCliente { get; set; }
        public decimal Hectareas { get; set; }
        public int EstadoSolicitud { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public decimal Total { get; set; }
        public decimal PrecioBase { get; set; }
        public decimal Ganancia { get; set; }
        public decimal PrecioConGanancia { get; set; }
        public decimal PrecioConRiesgo { get; set; }
    }
}
