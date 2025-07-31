using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using IntelliSoftAPIV2.Models;

namespace IntelliSoftAPI.Models;

public partial class TbPedido
{
    [Key]
    [Column("id_pedido")]
    public int IdPedido { get; set; }

    [Column("cotizacion_id")]
    public int CotizacionId { get; set; }

    [Column("fecha_pedido")]
    public DateTime? FechaPedido { get; set; }

    [Column("estatus")]
    public int Estatus { get; set; } = 1;

    public virtual TbCotizacion Cotizacion { get; set; } = null!;
    public virtual ICollection<TbInventarioInsumo> TbInventarioInsumos { get; set; } = new List<TbInventarioInsumo>();
}
