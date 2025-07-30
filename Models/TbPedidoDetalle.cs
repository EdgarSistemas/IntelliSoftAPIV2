using System;
using System.Collections.Generic;

namespace IntelliSoftAPI.Models;

public partial class TbPedidoDetalle
{
    public int IdPedidoDetalle { get; set; }

    public int PedidoId { get; set; }

    public int ProductoId { get; set; }

    public int? Cantidad { get; set; }

    public decimal? PrecioUnitario { get; set; }

    //public virtual TbPedido Pedido { get; set; } = null!;

    //public virtual TbProducto Producto { get; set; } = null!;
}
