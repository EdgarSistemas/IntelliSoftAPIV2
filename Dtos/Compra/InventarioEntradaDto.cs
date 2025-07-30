using System.ComponentModel.DataAnnotations;

namespace IntelliSoftAPIV2.Dtos.Compra
{
    public class InventarioEntradaDto
    {
        [Required]
        public int InsumoId { get; set; }

        [Required]
        public int CantidadUnidad { get; set; }  // cantidad en unidades base, no presentaciones

        [Required]
        public decimal CostoUnitario { get; set; } // precio por unidad base despues de descomposición
    }
}
