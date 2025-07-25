using System;
using System.Collections.Generic;

namespace IntelliSoftAPI.Models;

public partial class TbInsumo
{
    public int IdInsumo { get; set; }

    public string? Nombre { get; set; }

    public string? Descripcion { get; set; }

    public decimal? PrecioUnitario { get; set; }

    public int UnidadId { get; set; }

    public int? Estatus { get; set; }

    public virtual ICollection<TbCompraDetalle> TbCompraDetalles { get; set; } = new List<TbCompraDetalle>();

    public virtual ICollection<TbInventarioInsumo> TbInventarioInsumos { get; set; } = new List<TbInventarioInsumo>();

    public virtual ICollection<TbProductoInsumo> TbProductoInsumos { get; set; } = new List<TbProductoInsumo>();

    public virtual TbCatalogoUnidad Unidad { get; set; } = null!;
}
