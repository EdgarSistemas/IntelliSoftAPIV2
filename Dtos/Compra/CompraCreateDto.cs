using System.ComponentModel.DataAnnotations;

namespace IntelliSoftAPIV2.Dtos.Compra
{
    public class CompraCreateDto
    {
        [Required]
        public int ProveedorId { get; set; }

        public string? Observacion { get; set; }

        [Required]
        public List<CompraDetalleCreateDto> Detalles { get; set; } = new();
    }
}
