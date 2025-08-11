using IntelliSoftAPI.Models;
using IntelliSoftAPIV2.Dtos.Cotizacion;

namespace IntelliSoftAPIV2.Dtos.Pedidos
{
    public class PedidoResponseDto
    {
        public int IdPedido { get; set; }
        public int CotizacionId { get; set; }

        public string CotizacionClave { get; set; }
        public DateTime? FechaPedido { get; set; }
        public int? Estatus { get; set; }

        public string? ClienteId { get; set; }
        public string NombreCliente { get; set; } = "";

        public string? Comentario { get; set; }

        public List<CotizacionPartidaDto> Partidas { get; set; } = new();

        public decimal TotalPrecioBase { get; set; }
        public decimal TotalGanancia { get; set; }
        public decimal TotalPrecioConGanancia { get; set; }
        public decimal TotalPrecioConRiesgo { get; set; }
        public decimal Total { get; set; }
    }
}
