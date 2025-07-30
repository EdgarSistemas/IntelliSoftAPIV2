using System;
using System.Collections.Generic;

namespace IntelliSoftAPI.Models;

public partial class TbProducto
{
    public int IdProductos { get; set; }

    public string? Nombre { get; set; }

    public string? Descripcion { get; set; }

    public decimal? PrecioBase { get; set; }

    public decimal? HectareaBase { get; set; }

    public virtual ICollection<TbCotizacion> TbCotizaciones { get; set; } = new List<TbCotizacion>();

    public virtual ICollection<TbOpinion> TbOpiniones { get; set; } = new List<TbOpinion>();

    public virtual ICollection<TbProductoInsumo> TbProductoInsumos { get; set; } = new List<TbProductoInsumo>();
}
