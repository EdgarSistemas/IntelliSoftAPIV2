namespace IntelliSoftAPIV2.Dtos.Pedidos
{
    public class PedidoPartidaDto
    {
        public int CotizacionProductoId { get; set; }
        public int ProductoId { get; set; }
        public string? NombreProducto { get; set; }
        public decimal Hectareas { get; set; }

        public decimal PrecioVenta { get; set; }

        public List<PedidoPartidaInsumoDto> Detalles { get; set; } = new();
    }
}
