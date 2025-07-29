using System.ComponentModel.DataAnnotations;

namespace IntelliSoftAPIV2.Dtos.Users
{
    public class RegisterDto
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Direccion { get; set; }
        public string PhoneNumber { get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
        public string Rol { get; set; }
    }
}
