using IntelliSoftAPI.Models;
using IntelliSoftAPIV2.Dtos.Cotizacion;

namespace IntelliSoftAPIV2.Dtos.Pedidos
{
    public class PedidoResponseDto
    {
        public int IdPedido { get; set; }
        public int CotizacionId { get; set; }
        public DateTime? FechaPedido { get; set; }
        public int? Estatus { get; set; }

        public string? ClienteId { get; set; }
        public string NombreCliente { get; set; }

        public string? Comentario { get; set; }

        public int? ProductoId { get; set; }
        public string? NombreProducto { get; set; }
        public List<CotizacionDetalleDto>? Detalles { get; set; }
    }
}
