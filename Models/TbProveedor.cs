using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace IntelliSoftAPI.Models;

public partial class TbProveedor
{
    [Key]
    [Column("id_proveedor")]
    public int IdProveedor { get; set; }

    [Column("nombre")]
    [MaxLength(25)]
    public string? Nombre { get; set; }

    [Column("contacto")]
    [MaxLength(50)]
    public string? Contacto { get; set; }

    [Column("telefono")]
    [MaxLength(10)]
    public string? Telefono { get; set; }

    [Column("correo_electronico")]
    [MaxLength(50)]
    public string? CorreoElectronico { get; set; }

    [Column("descripcion_servicio")]
    [MaxLength(100)]
    public string? DescripcionServicio { get; set; }

    [Column("estatus")]
    public int Estatus { get; set; } = 1;

    public virtual ICollection<TbCompra> TbCompras { get; set; } = new List<TbCompra>();
}
