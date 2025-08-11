namespace IntelliSoftAPIV2.Dtos.Cotizacion
{
    public class CotizacionFullDto
    {
        public int IdCotizacion { get; set; }
        public string? ClaveCotizacion { get; set; }
        public string? UsuarioId { get; set; }
        public string? NombreCliente { get; set; }
        public int Estatus { get; set; }
        public DateTime? FechaSolicitud { get; set; }
        public string? DetalleCotizacion { get; set; }

        public List<CotizacionPartidaDto> Partidas { get; set; } = new();

        // totales consolidados (suma de partidas)
        public decimal TotalPrecioBase { get; set; }
        public decimal TotalGanancia { get; set; }
        public decimal TotalPrecioConGanancia { get; set; }
        public decimal TotalPrecioConRiesgo { get; set; }
        public decimal Total { get; set; }
    }
}
