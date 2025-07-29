using System.ComponentModel.DataAnnotations;

namespace IntelliSoftAPIV2.Dtos.Compra
{
    public class InventarioCreateDto
    {
        [Required]
        public int CompraId { get; set; }

        [Required]
        public List<InventarioEntradaDto> InsumosInventariados { get; set; } = new();
    }
}
