namespace IntelliSoftAPIV2.Dtos.Users
{
    public class UpdateUserDto
    {
        public string? Nombre { get; set; }
        public string? Apellidos { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? NewPassword { get; set; }   
        public string? CurrentPassword { get; set; }
    }
}
