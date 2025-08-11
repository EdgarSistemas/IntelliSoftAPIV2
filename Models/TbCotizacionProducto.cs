using IntelliSoftAPI.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using IntelliSoftAPIV2.Models;

public class TbCotizacionProducto
{
    [Key]
    [Column("id_cotizacion_producto")]
    public int IdCotizacionProducto { get; set; }

    [Column("cotizacion_id")]
    public int CotizacionId { get; set; }

    [Column("producto_id")]
    public int ProductoId { get; set; }

    [Column("hectareas")]
    public decimal Hectareas { get; set; }

    [Column("porcentaje_ganancia", TypeName = "decimal(5,2)")]
    public decimal PorcentajeGanancia { get; set; }

    [Column("porcentaje_riesgo", TypeName = "decimal(5,2)")]
    public decimal PorcentajeRiesgo { get; set; }

    [Column("aplica_riesgo")]
    public int AplicaRiesgo { get; set; } = 1;

    [ForeignKey(nameof(CotizacionId))]
    public TbCotizacion Cotizacion { get; set; } = null!;

    [ForeignKey(nameof(ProductoId))]
    public TbProducto Producto { get; set; } = null!;

    public ICollection<TbCotizacionProductoDetalle> Detalles { get; set; } = new List<TbCotizacionProductoDetalle>();
}
