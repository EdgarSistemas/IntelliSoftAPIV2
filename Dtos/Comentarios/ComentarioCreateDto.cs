using System.ComponentModel.DataAnnotations;

namespace IntelliSoftAPIV2.Dtos.Comentarios
{
    public class ComentarioOpinionCreateDto
    {
        [Required(ErrorMessage = "El mensaje es obligatorio.")]
        [StringLength(500, ErrorMessage = "El mensaje no puede exceder los 500 caracteres.")]
        public string Mensaje { get; set; }

        public string? UsuarioId { get; set; }
    }
}
