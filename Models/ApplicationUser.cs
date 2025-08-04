using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IntelliSoftAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace IntelliSoftAPIV2.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }
        [MaxLength(100)]
        public string? Apellidos { get; set; }
        public string? Direccion { get; set; }
        public DateTime fecha_registro { get; set; } = DateTime.UtcNow;
        public bool estatus { get; set; } = true;

        [Column("contrasena_generada")]
        [MaxLength(100)]
        public string? ContrasenaGenerada { get; set; }

        //Relaciones con las tablas
        public virtual ICollection<TbComentario> TbComentarios { get; set; }
        public virtual ICollection<TbCotizacion> TbCotizaciones { get; set; }
        public virtual ICollection<TbOpinion> TbOpiniones { get; set; }

    }
}
