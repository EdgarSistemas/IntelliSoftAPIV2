using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace IntelliSoftAPI.Models;

public partial class TbCompra
{
    [Key]
    [Column("id_compra")]
    public int IdCompra { get; set; }

    [MaxLength(65)]
    [Column("clave_compra")]
    public string? ClaveCompra { get; set; }

    [Column("fecha_compra")]
    public DateTime? FechaCompra { get; set; }

    [MaxLength(65)]
    [Column("observacion")]
    public string? Observacion { get; set; }

    [Column("estatus")]
    public int? Estatus { get; set; }

    [Required]
    [Column("proveedor_id")]
    public int ProveedorId { get; set; }

    [ForeignKey("ProveedorId")]
    public virtual TbProveedor Proveedor { get; set; } = null!;

    public virtual ICollection<TbCompraDetalle> TbCompraDetalles { get; set; } = new List<TbCompraDetalle>();

    public virtual ICollection<TbInventarioInsumo> TbInventarioInsumos { get; set; } = new List<TbInventarioInsumo>();
}
