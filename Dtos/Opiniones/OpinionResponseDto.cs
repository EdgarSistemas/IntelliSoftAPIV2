namespace IntelliSoftAPIV2.Dtos.Opiniones
{
    public class OpinionResponseDto
    {
        public int IdOpinion { get; set; }
        public string? UsuarioId { get; set; }
        public string UsuarioNombre { get; set; }
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; }
        public int Calificacion { get; set; }
        public string? Comentario { get; set; }
        public DateTime? Fecha { get; set; }
    }
}
