namespace IntelliSoftAPIV2.Dtos.Pedidos
{
    public class PedidoPartidaInsumoDto
    {
        public int InsumoId { get; set; }
        public string NombreInsumo { get; set; } = "";
        public decimal Cantidad { get; set; }
    }
}
