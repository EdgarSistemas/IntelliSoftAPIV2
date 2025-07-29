using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace IntelliSoftAPI.Models;

public partial class TbInventarioInsumo
{
    [Key]
    [Column("id_inventario_insumo")]
    public int IdInventarioInsumo { get; set; }

    [Required]
    [Column("insumo_id")]
    public int InsumoId { get; set; }

    [Column("fecha")]
    public DateTime? Fecha { get; set; }

    [Column("entrada")]
    public int? Entrada { get; set; }

    [Column("salida")]
    public int? Salida { get; set; }

    [Column("existencias")]
    public int? Existencias { get; set; }

    [Column("costo", TypeName = "decimal(18,2)")]
    public decimal? Costo { get; set; }

    [Column("promedio", TypeName = "decimal(18,2)")]
    public decimal? Promedio { get; set; }

    [Column("debe", TypeName = "decimal(18,2)")]
    public decimal? Debe { get; set; }

    [Column("haber", TypeName = "decimal(18,2)")]
    public decimal? Haber { get; set; }

    [Column("saldo", TypeName = "decimal(18,2)")]
    public decimal? Saldo { get; set; }

    [Column("compra_id")]
    public int? CompraId { get; set; }

    [Column("pedido_id")]
    public int? PedidoId { get; set; }

    [ForeignKey("CompraId")]
    public virtual TbCompra? Compra { get; set; }

    [ForeignKey("InsumoId")]
    public virtual TbInsumo Insumo { get; set; } = null!;

    [ForeignKey("PedidoId")]
    public virtual TbPedido? Pedido { get; set; }
}
