using System.ComponentModel.DataAnnotations;

namespace IntelliSoftAPIV2.Dtos.Cotizacion
{
    public class CotizacionCreateDto
    {
        [Required]
        public int ProductoId { get; set; }

        [Required]
        public decimal Hectareas { get; set; }

        // UsuarioId puede venir null si aún no se tiene (se verifica en backend según el email)
        public string? UsuarioId { get; set; }

        [Required]
        public List<CotizacionDetalleCreateDto> Detalles { get; set; } = new();
    }
}
