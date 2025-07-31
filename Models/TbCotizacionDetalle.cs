using IntelliSoftAPI.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace IntelliSoftAPIV2.Models
{
    public class TbCotizacionDetalle
    {
        [Key]
        [Column("id_cotizacion_detalle")]
        public int IdCotizacionDetalle { get; set; }

        [Column("cotizacion_id")]
        public int CotizacionId { get; set; }

        [Column("insumo_id")]
        public int InsumoId { get; set; }

        [Column("cantidad")]
        public decimal Cantidad { get; set; }

        [Column("precio_promedio", TypeName = "decimal(18, 2)")]
        public decimal PrecioPromedio { get; set; }

        [ForeignKey("CotizacionId")]
        public virtual TbCotizacion Cotizacion { get; set; } = null!;

        [ForeignKey("InsumoId")]
        public virtual TbInsumo Insumo { get; set; } = null!;
    }
}
