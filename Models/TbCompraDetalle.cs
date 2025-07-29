using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace IntelliSoftAPI.Models;

public partial class TbCompraDetalle
{
    [Key]
    [Column("id_compra_detalle")]
    public int IdCompraDetalle { get; set; }

    [Required]
    [Column("compra_id")]
    public int CompraId { get; set; }

    [Required]
    [Column("insumo_id")]
    public int InsumoId { get; set; }

    [MaxLength(50)]
    [Column("presentacion")]
    public string? Presentacion { get; set; }

    [Required]
    [Column("precio_unitario", TypeName = "decimal(18,2)")]
    public decimal PrecioUnitario { get; set; }

    [Required]
    [Column("cantidad")]
    public int Cantidad { get; set; }

    [ForeignKey("CompraId")]
    public virtual TbCompra Compra { get; set; } = null!;

    [ForeignKey("InsumoId")]
    public virtual TbInsumo Insumo { get; set; } = null!;
}
