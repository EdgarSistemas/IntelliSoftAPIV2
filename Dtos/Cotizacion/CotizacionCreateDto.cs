using System.ComponentModel.DataAnnotations;

namespace IntelliSoftAPIV2.Dtos.Cotizacion
{
    public class CotizacionCreateDto
    {
        public string? UsuarioId { get; set; }
        public string? DetalleCotizacion { get; set; }

        [Required]
        public List<CotizacionPartidaCreateDto> Partidas { get; set; } = new();
    }
}
