using System.ComponentModel.DataAnnotations;

namespace IntelliSoftAPIV2.Dtos.Compra
{
    public class InventariarCompraDto
    {
        [Required]
        public int CompraId { get; set; }

        [Required]
        public List<InventarioEntradaDto> Entradas { get; set; } = new();
    }
}
