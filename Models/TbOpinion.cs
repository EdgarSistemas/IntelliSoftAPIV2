using System;
using System.Collections.Generic;
using IntelliSoftAPIV2.Models;

namespace IntelliSoftAPI.Models;

public partial class TbOpinion
{
    public int IdOpinion { get; set; }

    public string? UsuarioId { get; set; }

    public int? ProductoId { get; set; }

    public int? Calificacion { get; set; }

    public string? Comentario { get; set; }

    public DateTime? Fecha { get; set; }
    public int Estatus { get; set; }

    public virtual ApplicationUser Usuario { get; set; } = null!;

    public virtual TbProducto? Producto { get; set; }

    public virtual ICollection<TbComentario> Comentarios { get; set; } = new List<TbComentario>();
}
