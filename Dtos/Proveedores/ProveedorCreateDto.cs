using System.ComponentModel.DataAnnotations;

namespace IntelliSoftAPIV2.Dtos.Proveedores
{
    public class ProveedorCreateDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "El teléfono debe tener 10 caracteres.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "El teléfono debe contener solo números.")]
        public string Telefono { get; set; }
        public string? Contacto { get; set; }
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        public string? CorreoElectronico { get; set; }
        public string? DescripcionServicio { get; set; }
        [Required]
        public int Estatus { get; set; }
    }
}
