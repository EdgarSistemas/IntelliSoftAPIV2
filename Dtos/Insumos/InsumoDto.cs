using IntelliSoftAPIV2.Dtos.Unidades;

namespace IntelliSoftAPIV2.Dtos.Insumo
{
    public class InsumoDto
    {
        public int IdInsumo { get; set; }
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public int UnidadId { get; set; }
        public UnidadDto Unidad { get; set; }
        public int? Estatus { get; set; }
        public int? Existencias { get; set; }
        public decimal? PrecioPromedio { get; set; }
    }
}
