using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace IntelliSoftAPI.Models;

public partial class TbCatalogoUnidad
{
    [Key]
    [Column("id_unidad")]
    public int IdUnidad { get; set; }

    [Required]
    [MaxLength(25)]
    [Column("nombre")]
    public string? Nombre { get; set; }

    [MaxLength(5)]
    [Column("simbolo")]
    public string? Simbolo { get; set; }

    [MaxLength(100)]
    [Column("descripcion")]
    public string? Descripcion { get; set; }

    [Column("estatus")]
    public int? Estatus { get; set; }

    public virtual ICollection<TbInsumo> TbInsumos { get; set; } = new List<TbInsumo>();
}
