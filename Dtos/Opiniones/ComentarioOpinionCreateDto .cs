using System.ComponentModel.DataAnnotations;

namespace IntelliSoftAPIV2.Dtos.Opiniones
{
    public class ComentarioCreateDto
    {
        [Required(ErrorMessage = "La opinión es requerida")]
        public int OpinionId { get; set; }

        [Required(ErrorMessage = "El mensaje del comentario es requerido")]
        public string Mensaje { get; set; }
    }
}
