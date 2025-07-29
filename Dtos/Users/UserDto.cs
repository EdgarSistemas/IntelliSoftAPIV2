using System.ComponentModel.DataAnnotations;

namespace IntelliSoftAPIV2.Dtos.Users
{
    public class UserDto
    {
        public int? Id { get; set; }

        public string Nombre{ get; set; }
        public string Apellidos { get; set; }
        public string Email { get; set; }
        public string? Direccion { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Rol { get; set; }
      
        

    }
}
