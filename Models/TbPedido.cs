using System;
using System.Collections.Generic;
using IntelliSoftAPIV2.Models;

namespace IntelliSoftAPI.Models;

public partial class TbPedido
{
    public int IdPedido { get; set; }

    public string? UsuarioId { get; set; }

    public DateTime? FechaPedido { get; set; }

    public decimal? Total { get; set; }

    public string? Estatus { get; set; }

    public virtual ApplicationUser Usuario { get; set; }

    public virtual ICollection<TbInventarioInsumo> TbInventarioInsumos { get; set; } = new List<TbInventarioInsumo>();

    public virtual ICollection<TbPedidoDetalle> TbPedidoDetalles { get; set; } = new List<TbPedidoDetalle>();
}
