using System;
using System.Collections.Generic;
using IntelliSoftAPIV2.Models;

namespace IntelliSoftAPI.Models;

public partial class TbCotizacion
{
    public int IdCotizaciones { get; set; }

    public int? ProductoId { get; set; }

    public string? NombreSolicitante { get; set; }

    public string? UsuarioId { get; set; }

    public string? EmailSolicitante { get; set; }

    public DateTime? FechaSolicitud { get; set; }

    public string? EstadoSolicitud { get; set; }

    public string? DetalleCotizacion { get; set; }

    public virtual ApplicationUser Usuario { get; set; }

    public virtual TbProducto? Producto { get; set; }

    public virtual ICollection<TbPedido> TbPedidos { get; set; } = new List<TbPedido>();
}
