namespace IntelliSoftAPIV2.Dtos.Cotizacion
{
    public class CotizacionConPartidasDto
    {
        public int IdCotizacion { get; set; }
        public string? ClaveCotizacion { get; set; }
        public string UsuarioId { get; set; } = string.Empty;
        public int Estatus { get; set; }
        public DateTime? FechaSolicitud { get; set; }
        public List<CotizacionProductoResumenDto> Partidas { get; set; } = new();
        public decimal TotalCotizacion { get; set; }
    }
}
