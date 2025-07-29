using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using IntelliSoftAPIV2.Models;

namespace IntelliSoftAPI.Models;

public partial class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TbCatalogoUnidad> TbCatalogoUnidads { get; set; }

    public virtual DbSet<TbComentario> TbComentarios { get; set; }

    public virtual DbSet<TbCompra> TbCompras { get; set; }

    public virtual DbSet<TbCompraDetalle> TbCompraDetalles { get; set; }

    public virtual DbSet<TbCotizacion> TbCotizaciones { get; set; }

    public virtual DbSet<TbInsumo> TbInsumos { get; set; }

    public virtual DbSet<TbInventarioInsumo> TbInventarioInsumos { get; set; }



    public virtual DbSet<TbOpinion> TbOpiniones { get; set; }

    public virtual DbSet<TbPedido> TbPedidos { get; set; }

    public virtual DbSet<TbPedidoDetalle> TbPedidoDetalles { get; set; }

    public virtual DbSet<TbProducto> TbProductos { get; set; }

    public virtual DbSet<TbProductoInsumo> TbProductoInsumos { get; set; }

    public virtual DbSet<TbProveedor> TbProveedors { get; set; }

    public virtual DbSet<ApplicationUser> ApplicationUser { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TbCatalogoUnidad>(entity =>
        {
            entity.HasKey(e => e.IdUnidad).HasName("PK__TB_Catal__95D7C92B47769009");

            entity.ToTable("TB_CatalogoUnidad", "catalogos");

            entity.Property(e => e.IdUnidad).HasColumnName("id_unidad");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Estatus).HasColumnName("estatus");
            entity.Property(e => e.Nombre)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Simbolo)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("simbolo");
        });

        modelBuilder.Entity<TbComentario>(entity =>
        {
            entity.HasKey(e => e.IdComentario).HasName("PK__TB_Comen__1BA6C6F42BCD59A9");

            entity.ToTable("TB_Comentarios", "seguridad");

            entity.Property(e => e.IdComentario).HasColumnName("id_comentario");
            entity.Property(e => e.Fecha)
                .HasColumnType("datetime")
                .HasColumnName("fecha");
            entity.Property(e => e.Mensaje)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("mensaje");
            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");

            entity.HasOne(d => d.Usuario).WithMany(p => p.TbComentarios)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("FK__TB_Coment__usuar__0C85DE4D");
        });

        modelBuilder.Entity<TbCompra>(entity =>
        {
            entity.HasKey(e => e.IdCompra).HasName("PK__TB_Compr__C4BAA604B65F62CC");

            entity.ToTable("TB_Compra", "almacen");

            entity.Property(e => e.IdCompra).HasColumnName("id_compra");
            entity.Property(e => e.ClaveCompra)
                .HasMaxLength(65)
                .IsUnicode(false)
                .HasColumnName("clave_compra");
            entity.Property(e => e.Estatus).HasColumnName("estatus");
            entity.Property(e => e.FechaCompra)
                .HasColumnType("datetime")
                .HasColumnName("fecha_compra");
            entity.Property(e => e.Observacion)
                .HasMaxLength(65)
                .IsUnicode(false)
                .HasColumnName("observacion");
            entity.Property(e => e.ProveedorId).HasColumnName("proveedor_id");

            entity.HasOne(d => d.Proveedor).WithMany(p => p.TbCompras)
                .HasForeignKey(d => d.ProveedorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TB_Compra__prove__7F2BE32F");
        });

        modelBuilder.Entity<TbCompraDetalle>(entity =>
        {
            entity.HasKey(e => e.IdCompraDetalle).HasName("PK__TB_Compr__C08AA00690F89481");

            entity.ToTable("TB_CompraDetalle", "almacen");

            entity.Property(e => e.IdCompraDetalle).HasColumnName("id_compra_detalle");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.CompraId).HasColumnName("compra_id");
            entity.Property(e => e.InsumoId).HasColumnName("insumo_id");
            entity.Property(e => e.PrecioUnitario)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("precio_unitario");
            entity.Property(e => e.Presentacion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("presentacion");

            entity.HasOne(d => d.Compra).WithMany(p => p.TbCompraDetalles)
                .HasForeignKey(d => d.CompraId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TB_Compra__compr__7D439ABD");

            entity.HasOne(d => d.Insumo).WithMany(p => p.TbCompraDetalles)
                .HasForeignKey(d => d.InsumoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TB_Compra__insum__7E37BEF6");
        });

        modelBuilder.Entity<TbCotizacion>(entity =>
        {
            entity.HasKey(e => e.IdCotizaciones).HasName("PK__TB_Cotiz__21B35BCEDB402AE2");

            entity.ToTable("TB_Cotizaciones", "operaciones");

            entity.Property(e => e.IdCotizaciones).HasColumnName("id_cotizaciones");
            entity.Property(e => e.UsuarioId).HasColumnName("cliente_id");
            entity.Property(e => e.DetalleCotizacion)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("detalle_cotizacion");
            entity.Property(e => e.EmailSolicitante)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("email_solicitante");
            entity.Property(e => e.EstadoSolicitud)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("estado_solicitud");
            entity.Property(e => e.FechaSolicitud)
                .HasColumnType("datetime")
                .HasColumnName("fecha_solicitud");
            entity.Property(e => e.NombreSolicitante)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("nombre_solicitante");
            entity.Property(e => e.ProductoId).HasColumnName("producto_id");

            entity.HasOne(d => d.Usuario).WithMany(p => p.TbCotizaciones)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("FK__TB_Cotiza__clien__07C12930");

            entity.HasOne(d => d.Producto).WithMany(p => p.TbCotizaciones)
                .HasForeignKey(d => d.ProductoId)
                .HasConstraintName("FK__TB_Cotiza__produ__06CD04F7");
        });

        modelBuilder.Entity<TbInsumo>(entity =>
        {
            entity.HasKey(e => e.IdInsumo).HasName("PK__TB_Insum__D4F202B1F9356D73");

            entity.ToTable("TB_Insumo", "catalogos");

            entity.Property(e => e.IdInsumo).HasColumnName("id_insumo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Estatus).HasColumnName("estatus");
            entity.Property(e => e.Nombre)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.PrecioUnitario)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("precio_unitario");
            entity.Property(e => e.UnidadId).HasColumnName("unidad_id");

            entity.HasOne(d => d.Unidad).WithMany(p => p.TbInsumos)
                .HasForeignKey(d => d.UnidadId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TB_Insumo__unida__02FC7413");
        });

        modelBuilder.Entity<TbInventarioInsumo>(entity =>
        {
            entity.HasKey(e => e.IdInventarioInsumo).HasName("PK__TB_Inven__D64F6112270E14D6");

            entity.ToTable("TB_InventarioInsumo", "almacen");

            entity.Property(e => e.IdInventarioInsumo)
                .HasColumnName("id_inventario_insumo")
                .ValueGeneratedOnAdd();
            entity.Property(e => e.CompraId).HasColumnName("compra_id");
            entity.Property(e => e.Costo)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("costo");
            entity.Property(e => e.Debe)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("debe");
            entity.Property(e => e.Entrada).HasColumnName("entrada");
            entity.Property(e => e.Existencias).HasColumnName("existencias");
            entity.Property(e => e.Fecha)
                .HasColumnType("datetime")
                .HasColumnName("fecha");
            entity.Property(e => e.Haber)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("haber");
            entity.Property(e => e.InsumoId).HasColumnName("insumo_id");
            entity.Property(e => e.PedidoId).HasColumnName("pedido_id");
            entity.Property(e => e.Promedio)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("promedio");
            entity.Property(e => e.Saldo)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("saldo");
            entity.Property(e => e.Salida).HasColumnName("salida");

            entity.HasOne(d => d.Compra).WithMany(p => p.TbInventarioInsumos)
                .HasForeignKey(d => d.CompraId)
                .HasConstraintName("FK__TB_Invent__compr__01142BA1");

            entity.HasOne(d => d.Insumo).WithMany(p => p.TbInventarioInsumos)
                .HasForeignKey(d => d.InsumoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TB_Invent__insum__02084FDA");

            entity.HasOne(d => d.Pedido).WithMany(p => p.TbInventarioInsumos)
                .HasForeignKey(d => d.PedidoId)
                .HasConstraintName("FK__TB_Invent__pedid__00200768");
        });

        modelBuilder.Entity<TbOpinion>(entity =>
        {
            entity.HasKey(e => e.IdOpinion).HasName("PK__TB_Opini__04DDBD783B2ACCAE");

            entity.ToTable("TB_Opiniones", "operaciones");

            entity.Property(e => e.IdOpinion).HasColumnName("id_opinion");
            entity.Property(e => e.Calificacion).HasColumnName("calificacion");
            entity.Property(e => e.UsuarioId).HasColumnName("cliente_id");
            entity.Property(e => e.Comentario)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("comentario");
            entity.Property(e => e.Fecha)
                .HasColumnType("datetime")
                .HasColumnName("fecha");
            entity.Property(e => e.ProductoId).HasColumnName("producto_id");

            entity.HasOne(d => d.Usuario).WithMany(p => p.TbOpiniones)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TB_Opinio__clien__09A971A2");

            entity.HasOne(d => d.Producto).WithMany(p => p.TbOpiniones)
                .HasForeignKey(d => d.ProductoId)
                .HasConstraintName("FK__TB_Opinio__produ__08B54D69");
        });

        modelBuilder.Entity<TbPedido>(entity =>
        {
            entity.HasKey(e => e.IdPedido).HasName("PK__TB_Pedid__6FF01489CC0F52C0");

            entity.ToTable("TB_Pedido", "operaciones");

            entity.Property(e => e.IdPedido).HasColumnName("id_pedido");
            entity.Property(e => e.UsuarioId).HasColumnName("cliente_id");
            entity.Property(e => e.Estatus)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("estatus");
            entity.Property(e => e.FechaPedido)
                .HasColumnType("datetime")
                .HasColumnName("fecha_pedido");
            entity.Property(e => e.Total)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("total");

            entity.HasOne(d => d.Usuario).WithMany(p => p.TbPedidos)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TB_Pedido__clien__05D8E0BE");
        });

        modelBuilder.Entity<TbPedidoDetalle>(entity =>
        {
            entity.HasKey(e => e.IdPedidoDetalle).HasName("PK__TB_Pedid__392B2DE9C5CADE03");

            entity.ToTable("TB_PedidoDetalle", "operaciones");

            entity.Property(e => e.IdPedidoDetalle).HasColumnName("id_pedido_detalle");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.PedidoId).HasColumnName("pedido_id");
            entity.Property(e => e.PrecioUnitario)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("precio_unitario");
            entity.Property(e => e.ProductoId).HasColumnName("producto_id");

            entity.HasOne(d => d.Pedido).WithMany(p => p.TbPedidoDetalles)
                .HasForeignKey(d => d.PedidoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TB_Pedido__pedid__0A9D95DB");

            entity.HasOne(d => d.Producto).WithMany(p => p.TbPedidoDetalles)
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TB_Pedido__produ__0B91BA14");
        });


        modelBuilder.Entity<TbProducto>(entity =>
        {
            entity.HasKey(e => e.IdProductos).HasName("PK__TB_Produ__3804F4FB0336751E");

            entity.ToTable("TB_Productos", "catalogos");

            entity.Property(e => e.IdProductos).HasColumnName("id_productos");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.HectareaBase)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("hectarea_base");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.PrecioBase)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio_base");
        });

        modelBuilder.Entity<TbProductoInsumo>(entity =>
        {
            entity.HasKey(e => e.IdProductoInsumo).HasName("PK__TB_Produ__E26856CC7C46A71C");

            entity.ToTable("TB_ProductoInsumo", "catalogos");

            entity.Property(e => e.IdProductoInsumo).HasColumnName("id_producto_insumo");
            entity.Property(e => e.Cantidad)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("cantidad");
            entity.Property(e => e.InsumoId).HasColumnName("insumo_id");
            entity.Property(e => e.ProductoId).HasColumnName("producto_id");

            entity.HasOne(d => d.Insumo).WithMany(p => p.TbProductoInsumos)
                .HasForeignKey(d => d.InsumoId)
                .HasConstraintName("FK__TB_Produc__insum__03F0984C");

            entity.HasOne(d => d.Producto).WithMany(p => p.TbProductoInsumos)
                .HasForeignKey(d => d.ProductoId)
                .HasConstraintName("FK__TB_Produc__produ__04E4BC85");
        });

        modelBuilder.Entity<TbProveedor>(entity =>
        {
            entity.HasKey(e => e.IdProveedor).HasName("PK__TB_Prove__8D3DFE28F215D3E6");

            entity.ToTable("TB_Proveedor", "catalogos");

            entity.Property(e => e.IdProveedor).HasColumnName("id_proveedor");
            entity.Property(e => e.Contacto)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("contacto");
            entity.Property(e => e.CorreoElectronico)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("correo_electronico");
            entity.Property(e => e.DescripcionServicio)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("descripcion_servicio");
            entity.Property(e => e.Estatus)
                .HasDefaultValue(1)
                .HasColumnName("estatus");
            entity.Property(e => e.Nombre)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Telefono)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("telefono");
        });
       

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
