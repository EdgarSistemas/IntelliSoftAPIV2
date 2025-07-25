using System;
using System.Collections.Generic;

namespace IntelliSoftAPI.Models;

public partial class TbInventarioInsumo
{
    public int IdInventarioInsumo { get; set; }

    public int InsumoId { get; set; }

    public DateTime? Fecha { get; set; }

    public int? Entrada { get; set; }

    public int? Salida { get; set; }

    public int? Existencias { get; set; }

    public decimal? Costo { get; set; }

    public decimal? Promedio { get; set; }

    public decimal? Debe { get; set; }

    public decimal? Haber { get; set; }

    public decimal? Saldo { get; set; }

    public int? CompraId { get; set; }

    public int? PedidoId { get; set; }

    public virtual TbCompra? Compra { get; set; }

    public virtual TbInsumo Insumo { get; set; } = null!;

    public virtual TbPedido? Pedido { get; set; }
}
