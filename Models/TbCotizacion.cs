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

    [Column("producto_id")]
    public int? ProductoId { get; set; }

    [Column("cliente_id")]
    public string? UsuarioId { get; set; }

    [Column("fecha_solicitud")]
    public DateTime? FechaSolicitud { get; set; }

    [Column("estatus")]
    public int Estatus { get; set; } = 1;

    [Column("detalle_cotizacion")]
    [MaxLength(200)]
    public string? DetalleCotizacion { get; set; }

    [Column("hectareas")]
    public decimal Hectareas { get; set; }

    [Column("porcentaje_ganancia", TypeName = "decimal(5,2)")]
    public decimal PorcentajeGanancia { get; set; }

    [Column("porcentaje_riesgo", TypeName = "decimal(5,2)")]
    public decimal PorcentajeRiesgo { get; set; }

    [Column("aplica_riesgo")]
    public int AplicaRiesgo { get; set; }

    public virtual ApplicationUser Usuario { get; set; } = null!;
    public virtual TbProducto? Producto { get; set; }

    public virtual ICollection<TbPedido> TbPedidos { get; set; } = new List<TbPedido>();
    public virtual ICollection<TbCotizacionDetalle> Detalles { get; set; } = new List<TbCotizacionDetalle>();
}
