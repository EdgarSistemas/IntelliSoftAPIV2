using System.ComponentModel.DataAnnotations;

namespace IntelliSoftAPIV2.Dtos.Cotizacion
{
    public class CotizacionCreateDto
    {
        [Required]
        public int ProductoId { get; set; }

        [Required]
        public decimal Hectareas { get; set; }

        public string? UsuarioId { get; set; }

        public string? DetalleCotizacion { get; set; }
    }
}
