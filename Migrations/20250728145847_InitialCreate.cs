using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntelliSoftAPIV2.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "catalogos");

            migrationBuilder.EnsureSchema(
                name: "seguridad");

            migrationBuilder.EnsureSchema(
                name: "almacen");

            migrationBuilder.EnsureSchema(
                name: "operaciones");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellidos = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fecha_registro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    estatus = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TB_CatalogoUnidad",
                schema: "catalogos",
                columns: table => new
                {
                    id_unidad = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "varchar(25)", unicode: false, maxLength: 25, nullable: true),
                    simbolo = table.Column<string>(type: "varchar(5)", unicode: false, maxLength: 5, nullable: true),
                    descripcion = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    estatus = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TB_Catal__95D7C92B47769009", x => x.id_unidad);
                });

            migrationBuilder.CreateTable(
                name: "TB_Productos",
                schema: "catalogos",
                columns: table => new
                {
                    id_productos = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    descripcion = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    precio_base = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    hectarea_base = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TB_Produ__3804F4FB0336751E", x => x.id_productos);
                });

            migrationBuilder.CreateTable(
                name: "TB_Proveedor",
                schema: "catalogos",
                columns: table => new
                {
                    id_proveedor = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "varchar(25)", unicode: false, maxLength: 25, nullable: false),
                    telefono = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    contacto = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    correo_electronico = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    descripcion_servicio = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    estatus = table.Column<int>(type: "int", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TB_Prove__8D3DFE28F215D3E6", x => x.id_proveedor);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TB_Comentarios",
                schema: "seguridad",
                columns: table => new
                {
                    id_comentario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    mensaje = table.Column<string>(type: "varchar(1000)", unicode: false, maxLength: 1000, nullable: true),
                    fecha = table.Column<DateTime>(type: "datetime", nullable: true),
                    usuario_id = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TB_Comen__1BA6C6F42BCD59A9", x => x.id_comentario);
                    table.ForeignKey(
                        name: "FK__TB_Coment__usuar__0C85DE4D",
                        column: x => x.usuario_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TB_Pedido",
                schema: "operaciones",
                columns: table => new
                {
                    id_pedido = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cliente_id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    fecha_pedido = table.Column<DateTime>(type: "datetime", nullable: true),
                    total = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    estatus = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TB_Pedid__6FF01489CC0F52C0", x => x.id_pedido);
                    table.ForeignKey(
                        name: "FK__TB_Pedido__clien__05D8E0BE",
                        column: x => x.cliente_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TB_Insumo",
                schema: "catalogos",
                columns: table => new
                {
                    id_insumo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "varchar(25)", unicode: false, maxLength: 25, nullable: true),
                    descripcion = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    precio_unitario = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    unidad_id = table.Column<int>(type: "int", nullable: false),
                    estatus = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TB_Insum__D4F202B1F9356D73", x => x.id_insumo);
                    table.ForeignKey(
                        name: "FK__TB_Insumo__unida__02FC7413",
                        column: x => x.unidad_id,
                        principalSchema: "catalogos",
                        principalTable: "TB_CatalogoUnidad",
                        principalColumn: "id_unidad");
                });

            migrationBuilder.CreateTable(
                name: "TB_Cotizaciones",
                schema: "operaciones",
                columns: table => new
                {
                    id_cotizaciones = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    producto_id = table.Column<int>(type: "int", nullable: true),
                    nombre_solicitante = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    cliente_id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    email_solicitante = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    fecha_solicitud = table.Column<DateTime>(type: "datetime", nullable: true),
                    estado_solicitud = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    detalle_cotizacion = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TB_Cotiz__21B35BCEDB402AE2", x => x.id_cotizaciones);
                    table.ForeignKey(
                        name: "FK__TB_Cotiza__clien__07C12930",
                        column: x => x.cliente_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__TB_Cotiza__produ__06CD04F7",
                        column: x => x.producto_id,
                        principalSchema: "catalogos",
                        principalTable: "TB_Productos",
                        principalColumn: "id_productos");
                });

            migrationBuilder.CreateTable(
                name: "TB_Opiniones",
                schema: "operaciones",
                columns: table => new
                {
                    id_opinion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cliente_id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    producto_id = table.Column<int>(type: "int", nullable: true),
                    calificacion = table.Column<int>(type: "int", nullable: true),
                    comentario = table.Column<string>(type: "varchar(1000)", unicode: false, maxLength: 1000, nullable: true),
                    fecha = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TB_Opini__04DDBD783B2ACCAE", x => x.id_opinion);
                    table.ForeignKey(
                        name: "FK__TB_Opinio__clien__09A971A2",
                        column: x => x.cliente_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__TB_Opinio__produ__08B54D69",
                        column: x => x.producto_id,
                        principalSchema: "catalogos",
                        principalTable: "TB_Productos",
                        principalColumn: "id_productos");
                });

            migrationBuilder.CreateTable(
                name: "TB_Compra",
                schema: "almacen",
                columns: table => new
                {
                    id_compra = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    clave_compra = table.Column<string>(type: "varchar(65)", unicode: false, maxLength: 65, nullable: true),
                    fecha_compra = table.Column<DateTime>(type: "datetime", nullable: true),
                    observacion = table.Column<string>(type: "varchar(65)", unicode: false, maxLength: 65, nullable: true),
                    estatus = table.Column<int>(type: "int", nullable: true),
                    proveedor_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TB_Compr__C4BAA604B65F62CC", x => x.id_compra);
                    table.ForeignKey(
                        name: "FK__TB_Compra__prove__7F2BE32F",
                        column: x => x.proveedor_id,
                        principalSchema: "catalogos",
                        principalTable: "TB_Proveedor",
                        principalColumn: "id_proveedor");
                });

            migrationBuilder.CreateTable(
                name: "TB_PedidoDetalle",
                schema: "operaciones",
                columns: table => new
                {
                    id_pedido_detalle = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    pedido_id = table.Column<int>(type: "int", nullable: false),
                    producto_id = table.Column<int>(type: "int", nullable: false),
                    cantidad = table.Column<int>(type: "int", nullable: true),
                    precio_unitario = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TB_Pedid__392B2DE9C5CADE03", x => x.id_pedido_detalle);
                    table.ForeignKey(
                        name: "FK__TB_Pedido__pedid__0A9D95DB",
                        column: x => x.pedido_id,
                        principalSchema: "operaciones",
                        principalTable: "TB_Pedido",
                        principalColumn: "id_pedido");
                    table.ForeignKey(
                        name: "FK__TB_Pedido__produ__0B91BA14",
                        column: x => x.producto_id,
                        principalSchema: "catalogos",
                        principalTable: "TB_Productos",
                        principalColumn: "id_productos");
                });

            migrationBuilder.CreateTable(
                name: "TB_ProductoInsumo",
                schema: "catalogos",
                columns: table => new
                {
                    id_producto_insumo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    producto_id = table.Column<int>(type: "int", nullable: true),
                    insumo_id = table.Column<int>(type: "int", nullable: true),
                    cantidad = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TB_Produ__E26856CC7C46A71C", x => x.id_producto_insumo);
                    table.ForeignKey(
                        name: "FK__TB_Produc__insum__03F0984C",
                        column: x => x.insumo_id,
                        principalSchema: "catalogos",
                        principalTable: "TB_Insumo",
                        principalColumn: "id_insumo");
                    table.ForeignKey(
                        name: "FK__TB_Produc__produ__04E4BC85",
                        column: x => x.producto_id,
                        principalSchema: "catalogos",
                        principalTable: "TB_Productos",
                        principalColumn: "id_productos");
                });

            migrationBuilder.CreateTable(
                name: "TB_CompraDetalle",
                schema: "almacen",
                columns: table => new
                {
                    id_compra_detalle = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    compra_id = table.Column<int>(type: "int", nullable: false),
                    insumo_id = table.Column<int>(type: "int", nullable: false),
                    presentacion = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    precio_unitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    cantidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TB_Compr__C08AA00690F89481", x => x.id_compra_detalle);
                    table.ForeignKey(
                        name: "FK__TB_Compra__compr__7D439ABD",
                        column: x => x.compra_id,
                        principalSchema: "almacen",
                        principalTable: "TB_Compra",
                        principalColumn: "id_compra");
                    table.ForeignKey(
                        name: "FK__TB_Compra__insum__7E37BEF6",
                        column: x => x.insumo_id,
                        principalSchema: "catalogos",
                        principalTable: "TB_Insumo",
                        principalColumn: "id_insumo");
                });

            migrationBuilder.CreateTable(
                name: "TB_InventarioInsumo",
                schema: "almacen",
                columns: table => new
                {
                    id_inventario_insumo = table.Column<int>(type: "int", nullable: false),
                    insumo_id = table.Column<int>(type: "int", nullable: false),
                    fecha = table.Column<DateTime>(type: "datetime", nullable: true),
                    entrada = table.Column<int>(type: "int", nullable: true),
                    salida = table.Column<int>(type: "int", nullable: true),
                    existencias = table.Column<int>(type: "int", nullable: true),
                    costo = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    promedio = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    debe = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    haber = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    saldo = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    compra_id = table.Column<int>(type: "int", nullable: true),
                    pedido_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TB_Inven__D64F6112270E14D6", x => x.id_inventario_insumo);
                    table.ForeignKey(
                        name: "FK__TB_Invent__compr__01142BA1",
                        column: x => x.compra_id,
                        principalSchema: "almacen",
                        principalTable: "TB_Compra",
                        principalColumn: "id_compra");
                    table.ForeignKey(
                        name: "FK__TB_Invent__insum__02084FDA",
                        column: x => x.insumo_id,
                        principalSchema: "catalogos",
                        principalTable: "TB_Insumo",
                        principalColumn: "id_insumo");
                    table.ForeignKey(
                        name: "FK__TB_Invent__pedid__00200768",
                        column: x => x.pedido_id,
                        principalSchema: "operaciones",
                        principalTable: "TB_Pedido",
                        principalColumn: "id_pedido");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TB_Comentarios_usuario_id",
                schema: "seguridad",
                table: "TB_Comentarios",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_TB_Compra_proveedor_id",
                schema: "almacen",
                table: "TB_Compra",
                column: "proveedor_id");

            migrationBuilder.CreateIndex(
                name: "IX_TB_CompraDetalle_compra_id",
                schema: "almacen",
                table: "TB_CompraDetalle",
                column: "compra_id");

            migrationBuilder.CreateIndex(
                name: "IX_TB_CompraDetalle_insumo_id",
                schema: "almacen",
                table: "TB_CompraDetalle",
                column: "insumo_id");

            migrationBuilder.CreateIndex(
                name: "IX_TB_Cotizaciones_cliente_id",
                schema: "operaciones",
                table: "TB_Cotizaciones",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "IX_TB_Cotizaciones_producto_id",
                schema: "operaciones",
                table: "TB_Cotizaciones",
                column: "producto_id");

            migrationBuilder.CreateIndex(
                name: "IX_TB_Insumo_unidad_id",
                schema: "catalogos",
                table: "TB_Insumo",
                column: "unidad_id");

            migrationBuilder.CreateIndex(
                name: "IX_TB_InventarioInsumo_compra_id",
                schema: "almacen",
                table: "TB_InventarioInsumo",
                column: "compra_id");

            migrationBuilder.CreateIndex(
                name: "IX_TB_InventarioInsumo_insumo_id",
                schema: "almacen",
                table: "TB_InventarioInsumo",
                column: "insumo_id");

            migrationBuilder.CreateIndex(
                name: "IX_TB_InventarioInsumo_pedido_id",
                schema: "almacen",
                table: "TB_InventarioInsumo",
                column: "pedido_id");

            migrationBuilder.CreateIndex(
                name: "IX_TB_Opiniones_cliente_id",
                schema: "operaciones",
                table: "TB_Opiniones",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "IX_TB_Opiniones_producto_id",
                schema: "operaciones",
                table: "TB_Opiniones",
                column: "producto_id");

            migrationBuilder.CreateIndex(
                name: "IX_TB_Pedido_cliente_id",
                schema: "operaciones",
                table: "TB_Pedido",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "IX_TB_PedidoDetalle_pedido_id",
                schema: "operaciones",
                table: "TB_PedidoDetalle",
                column: "pedido_id");

            migrationBuilder.CreateIndex(
                name: "IX_TB_PedidoDetalle_producto_id",
                schema: "operaciones",
                table: "TB_PedidoDetalle",
                column: "producto_id");

            migrationBuilder.CreateIndex(
                name: "IX_TB_ProductoInsumo_insumo_id",
                schema: "catalogos",
                table: "TB_ProductoInsumo",
                column: "insumo_id");

            migrationBuilder.CreateIndex(
                name: "IX_TB_ProductoInsumo_producto_id",
                schema: "catalogos",
                table: "TB_ProductoInsumo",
                column: "producto_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "TB_Comentarios",
                schema: "seguridad");

            migrationBuilder.DropTable(
                name: "TB_CompraDetalle",
                schema: "almacen");

            migrationBuilder.DropTable(
                name: "TB_Cotizaciones",
                schema: "operaciones");

            migrationBuilder.DropTable(
                name: "TB_InventarioInsumo",
                schema: "almacen");

            migrationBuilder.DropTable(
                name: "TB_Opiniones",
                schema: "operaciones");

            migrationBuilder.DropTable(
                name: "TB_PedidoDetalle",
                schema: "operaciones");

            migrationBuilder.DropTable(
                name: "TB_ProductoInsumo",
                schema: "catalogos");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "TB_Compra",
                schema: "almacen");

            migrationBuilder.DropTable(
                name: "TB_Pedido",
                schema: "operaciones");

            migrationBuilder.DropTable(
                name: "TB_Insumo",
                schema: "catalogos");

            migrationBuilder.DropTable(
                name: "TB_Productos",
                schema: "catalogos");

            migrationBuilder.DropTable(
                name: "TB_Proveedor",
                schema: "catalogos");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "TB_CatalogoUnidad",
                schema: "catalogos");
        }
    }
}
