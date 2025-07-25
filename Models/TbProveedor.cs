using System;
using System.Collections.Generic;

namespace IntelliSoftAPI.Models;

public partial class TbProveedor
{
    public int IdProveedor { get; set; }

    public string Nombre { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public string? Contacto { get; set; }

    public string? CorreoElectronico { get; set; }

    public string? DescripcionServicio { get; set; }

    public int Estatus { get; set; }

    public virtual ICollection<TbCompra> TbCompras { get; set; } = new List<TbCompra>();
}
