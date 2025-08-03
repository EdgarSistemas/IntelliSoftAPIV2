namespace IntelliSoftAPIV2.Dtos.Dashboard
{
    public class ProductoOpinionDto
    {
        public string NombreProducto { get; set; } = string.Empty;
        public double PromedioCalificacion { get; set; }
        public int TotalOpiniones { get; set; }
    }
}
