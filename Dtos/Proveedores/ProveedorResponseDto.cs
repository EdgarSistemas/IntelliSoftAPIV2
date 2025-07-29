namespace IntelliSoftAPIV2.Dtos.Proveedores
{
    public class ProveedorResponseDto
    {
        public int IdProveedor { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string? Contacto { get; set; }
        public string? CorreoElectronico { get; set; }
        public string? DescripcionServicio { get; set; }
        public int Estatus { get; set; }
    }
}
