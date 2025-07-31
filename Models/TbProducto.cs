using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace IntelliSoftAPI.Models;

public partial class TbProducto
{
    [Key]
    [Column("id_productos")]
    public int IdProductos { get; set; }

    [Column("nombre")]
    [MaxLength(100)]
    public string? Nombre { get; set; }

    [Column("descripcion")]
    [MaxLength(200)]
    public string? Descripcion { get; set; }

    [NotMapped]
    public decimal? PrecioBase { get; set; }

    [Column("hectarea_base", TypeName = "decimal(10,2)")]
    public decimal? HectareaBase { get; set; }

    [Column("estatus")]
    public int Estatus { get; set; } = 1;

    public virtual ICollection<TbCotizacion> TbCotizaciones { get; set; } = new List<TbCotizacion>();

    public virtual ICollection<TbOpinion> TbOpiniones { get; set; } = new List<TbOpinion>();

    public virtual ICollection<TbProductoInsumo> TbProductoInsumos { get; set; } = new List<TbProductoInsumo>();
}
