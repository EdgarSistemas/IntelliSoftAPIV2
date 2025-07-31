using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace IntelliSoftAPI.Models;

public partial class TbProductoInsumo
{
    [Key]
    [Column("id_producto_insumo")]
    public int IdProductoInsumo { get; set; }

    [Column("producto_id")]
    public int? ProductoId { get; set; }

    [Column("insumo_id")]
    public int? InsumoId { get; set; }

    [Column("cantidad", TypeName = "decimal(10,2)")]
    public decimal? Cantidad { get; set; }

    [ForeignKey("InsumoId")]
    public virtual TbInsumo? Insumo { get; set; }

    [ForeignKey("ProductoId")]
    public virtual TbProducto? Producto { get; set; }
}
