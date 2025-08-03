using IntelliSoftAPI.Models;

namespace IntelliSoftAPIV2.Models
{
    public class TbDocumento
    {
        public int IdDocumento { get; set; }
        public int IdProductos { get; set; }
        public string NombreDocumento { get; set; }
        public string Url {  get; set; }

        public virtual TbProducto? Producto { get; set; }
    }
}
