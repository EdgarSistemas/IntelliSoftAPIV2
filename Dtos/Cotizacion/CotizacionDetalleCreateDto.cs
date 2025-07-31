using System.ComponentModel.DataAnnotations;

namespace IntelliSoftAPIV2.Dtos.Cotizacion
{
    public class CotizacionDetalleCreateDto
    {
        [Required]
        public int InsumoId { get; set; }

        [Required]
        public decimal Cantidad { get; set; }

        [Required]
        public decimal PrecioPromedio { get; set; }
    }
}
