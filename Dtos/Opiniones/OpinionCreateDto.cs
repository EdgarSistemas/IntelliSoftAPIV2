using System.ComponentModel.DataAnnotations;

namespace IntelliSoftAPIV2.Dtos.Opiniones
{
    public class OpinionCreateDto
    {
        [Required(ErrorMessage = "El campo es requerido")]
        public string UsuarioId { get; set; }

        [Required(ErrorMessage = "El campo es requerido")]
        public int ProductoId { get; set; }

        [Range(1, 5, ErrorMessage = "La calificación debe estar entre 1 y 5.")]
        public int Calificacion { get; set; }

        public string? Comentario { get; set; }
    }
}
