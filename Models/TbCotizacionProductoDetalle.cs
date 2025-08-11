using IntelliSoftAPI.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace IntelliSoftAPIV2.Models
{
    public class TbCotizacionProductoDetalle
    {
        [Key]
        [Column("id_cotizacion_producto_detalle")]
        public int IdCotizacionProductoDetalle { get; set; }

        [Column("cotizacion_producto_id")]
        public int CotizacionProductoId { get; set; }

        [Column("insumo_id")]
        public int InsumoId { get; set; }

        [Column("cantidad")]
        public decimal Cantidad { get; set; }

        [Column("precio_promedio", TypeName = "decimal(18,2)")]
        public decimal PrecioPromedio { get; set; }

        [ForeignKey(nameof(CotizacionProductoId))]
        public TbCotizacionProducto CotizacionProducto { get; set; } = null!;

        [ForeignKey(nameof(InsumoId))]
        public TbInsumo Insumo { get; set; } = null!;
    }

}
