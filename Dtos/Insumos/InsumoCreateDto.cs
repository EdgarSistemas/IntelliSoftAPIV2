using System.ComponentModel.DataAnnotations;

namespace IntelliSoftAPIV2.Dtos.Insumos
{
    public class InsumoCreateDto
    {
        [Required]
        [MaxLength(25)]
        public string Nombre { get; set; }

        [MaxLength(100)]
        public string? Descripcion { get; set; }

        [Required]
        public int UnidadId { get; set; }
    }
}
