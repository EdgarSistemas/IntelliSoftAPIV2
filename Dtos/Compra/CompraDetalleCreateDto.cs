using System.ComponentModel.DataAnnotations;

namespace IntelliSoftAPIV2.Dtos.Compra
{
    public class CompraDetalleCreateDto
    {
        [Required]
        public int InsumoId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Presentacion { get; set; } = string.Empty;

        [Required]
        public decimal PrecioUnitario { get; set; }

        [Required]
        public int Cantidad { get; set; }
    }
}
