using System;
using System.Collections.Generic;

namespace IntelliSoftAPI.Models;

public partial class TbCompra
{
    public int IdCompra { get; set; }

    public string? ClaveCompra { get; set; }

    public DateTime? FechaCompra { get; set; }

    public string? Observacion { get; set; }

    public int? Estatus { get; set; }

    public int ProveedorId { get; set; }

    public virtual TbProveedor Proveedor { get; set; } = null!;

    public virtual ICollection<TbCompraDetalle> TbCompraDetalles { get; set; } = new List<TbCompraDetalle>();

    public virtual ICollection<TbInventarioInsumo> TbInventarioInsumos { get; set; } = new List<TbInventarioInsumo>();
}
