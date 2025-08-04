using System.ComponentModel.DataAnnotations;

namespace IntelliSoftAPIV2.Dtos.Users
{
    public class RegisterDto
    {
        public string? Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        public string Rol { get; set; }
    }
}
