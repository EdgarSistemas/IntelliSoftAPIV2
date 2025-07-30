using System;
using System.Collections.Generic;
using IntelliSoftAPIV2.Models;

namespace IntelliSoftAPI.Models;

public partial class TbPedido
{
    public int IdPedido { get; set; }

    public int CotizacionId { get; set; }

    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }

    public DateTime? FechaPedido { get; set; }

    public int? Estatus { get; set; } = 1;

    public virtual TbCotizacion Cotizacion { get; set; } = null!;

    public virtual ICollection<TbInventarioInsumo> TbInventarioInsumos { get; set; } = new List<TbInventarioInsumo>();
}
