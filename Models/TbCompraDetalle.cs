using System;
using System.Collections.Generic;

namespace IntelliSoftAPI.Models;

public partial class TbCompraDetalle
{
    public int IdCompraDetalle { get; set; }

    public int CompraId { get; set; }

    public int InsumoId { get; set; }

    public string? Presentacion { get; set; }

    public decimal PrecioUnitario { get; set; }

    public int Cantidad { get; set; }

    public virtual TbCompra Compra { get; set; } = null!;

    public virtual TbInsumo Insumo { get; set; } = null!;
}
