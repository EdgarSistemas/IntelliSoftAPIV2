using System;
using System.Collections.Generic;
using IntelliSoftAPIV2.Models;

namespace IntelliSoftAPI.Models;

public partial class TbComentario
{
    public int IdComentario { get; set; }
    public string? Mensaje { get; set; }

    public DateTime? Fecha { get; set; }

    public string? UsuarioId { get; set; }

    public int Estatus { get; set; }
    public virtual ApplicationUser Usuario { get; set; }
}
