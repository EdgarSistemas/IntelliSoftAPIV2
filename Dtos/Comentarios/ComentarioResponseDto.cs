namespace IntelliSoftAPIV2.Dtos.Comentarios
{
    public class ComentarioResponseDto
    {
        public int IdComentario { get; set; }
        public string Mensaje { get; set; }
        public DateTime? Fecha { get; set; }
        public string? UsuarioId { get; set; }
        public string? NombreUsuario { get; set; }
    }
}
