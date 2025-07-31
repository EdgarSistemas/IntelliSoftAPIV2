namespace IntelliSoftAPIV2.Dtos.Cotizacion
{
    public class CotizacionDetalleDto
    {
        public int InsumoId { get; set; }
        public string NombreInsumo { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioPromedio { get; set; }
        public decimal Subtotal => Cantidad * PrecioPromedio;
    }
}
