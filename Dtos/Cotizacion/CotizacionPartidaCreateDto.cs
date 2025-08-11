using System.ComponentModel.DataAnnotations;

namespace IntelliSoftAPIV2.Dtos.Cotizacion
{
    public class CotizacionPartidaCreateDto
    {
        [Required]
        public int ProductoId { get; set; }

        [Required]
        public decimal Hectareas { get; set; }
    }

}
