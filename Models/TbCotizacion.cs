using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using IntelliSoftAPIV2.Models;

namespace IntelliSoftAPI.Models;

public partial class TbCotizacion
{
    [Key]
    [Column("id_cotizaciones")]
    public int IdCotizaciones { get; set; }

    [MaxLength(65)]
    [Column("clave_cotizacion")]
    public string? ClaveCotizacion { get; set; }

    [Column("cliente_id")]
    public string? UsuarioId { get; set; }

    [Column("fecha_solicitud")]
    public DateTime? FechaSolicitud { get; set; }

    [Column("estatus")]
    public int Estatus { get; set; } = 1;

    [Column("detalle_cotizacion")]
    [MaxLength(200)]
    public string? DetalleCotizacion { get; set; }

    public virtual ApplicationUser Usuario { get; set; } = null!;

    public virtual ICollection<TbPedido> TbPedidos { get; set; } = new List<TbPedido>();

    // Partidas por producto dentro de la cotización
    public virtual ICollection<TbCotizacionProducto> CotizacionProductos { get; set; } = new List<TbCotizacionProducto>();
}
