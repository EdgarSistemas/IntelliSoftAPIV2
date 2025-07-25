namespace IntelliSoftAPIV2.Dtos.Cotizacion
{
    public class CotizacionResponseDto
    {
        public int Id { get; set; }
        public string? Detalle_cotizacion { get; set; }
        public decimal Total { get; set; }
        public DateTime Fecha_solicitud { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}
