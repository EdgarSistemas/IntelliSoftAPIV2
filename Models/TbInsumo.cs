using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace IntelliSoftAPI.Models;

public partial class TbInsumo
{
    [Key]
    [Column("id_insumo")]
    public int IdInsumo { get; set; }

    [Column("nombre")]
    [MaxLength(25)]
    public string? Nombre { get; set; }

    [Column("descripcion")]
    [MaxLength(100)]
    public string? Descripcion { get; set; }

    [NotMapped]
    public decimal? PrecioUnitario { get; set; }

    [Required]
    [Column("unidad_id")]
    public int UnidadId { get; set; }

    [Column("estatus")]
    public int? Estatus { get; set; }

    public virtual ICollection<TbCompraDetalle> TbCompraDetalles { get; set; } = new List<TbCompraDetalle>();

    public virtual ICollection<TbInventarioInsumo> TbInventarioInsumos { get; set; } = new List<TbInventarioInsumo>();

    public virtual ICollection<TbProductoInsumo> TbProductoInsumos { get; set; } = new List<TbProductoInsumo>();

    public virtual TbCatalogoUnidad Unidad { get; set; } = null!;
}
