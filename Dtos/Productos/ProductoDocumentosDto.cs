namespace IntelliSoftAPIV2.Dtos.Productos
{
    public class ProductoDocumentosDto
    {
        public int IdProducto { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }

        public List<DocumentoDto> Documentos { get; set; } = new();
    }
    public class DocumentoDto
    {
        public int IdDocumento { get; set; }
        public string NombreDocumento { get; set; }
        public string Url { get; set; }
    }
}
