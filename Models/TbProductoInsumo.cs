using System;
using System.Collections.Generic;

namespace IntelliSoftAPI.Models;

public partial class TbProductoInsumo
{
    public int IdProductoInsumo { get; set; }

    public int? ProductoId { get; set; }

    public int? InsumoId { get; set; }

    public decimal? Cantidad { get; set; }

    public virtual TbInsumo? Insumo { get; set; }

    public virtual TbProducto? Producto { get; set; }
}
