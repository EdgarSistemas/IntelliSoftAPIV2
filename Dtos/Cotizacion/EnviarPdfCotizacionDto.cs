namespace IntelliSoftAPIV2.Dtos.Cotizacion
{
    public class EnviarPdfCotizacionDto
    {
        public int IdCotizacion { get; set; }
        public string? Destinatario { get; set; }
        public string? Asunto { get; set; }
        public string? CuerpoHtml { get; set; }
    }
}
