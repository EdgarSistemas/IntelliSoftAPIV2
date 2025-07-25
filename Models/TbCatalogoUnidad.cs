using System;
using System.Collections.Generic;

namespace IntelliSoftAPI.Models;

public partial class TbCatalogoUnidad
{
    public int IdUnidad { get; set; }

    public string? Nombre { get; set; }

    public string? Simbolo { get; set; }

    public string? Descripcion { get; set; }

    public int? Estatus { get; set; }

    public virtual ICollection<TbInsumo> TbInsumos { get; set; } = new List<TbInsumo>();
}
