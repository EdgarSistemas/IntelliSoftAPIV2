using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntelliSoftAPIV2.Migrations
{
    /// <inheritdoc />
    public partial class ReconfigureCotizacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__TB_Cotiza__produ__06CD04F7",
                schema: "operaciones",
                table: "TB_Cotizaciones");

            migrationBuilder.DropTable(
                name: "TB_CotizacionDetalle",
                schema: "operaciones");

            migrationBuilder.DropIndex(
                name: "IX_TB_Cotizaciones_producto_id",
                schema: "operaciones",
                table: "TB_Cotizaciones");

            migrationBuilder.DropColumn(
                name: "precio_base",
                schema: "catalogos",
                table: "TB_Productos");

            migrationBuilder.DropColumn(
                name: "precio_unitario",
                schema: "catalogos",
                table: "TB_Insumo");

            migrationBuilder.DropColumn(
                name: "aplica_riesgo",
                schema: "operaciones",
                table: "TB_Cotizaciones");

            migrationBuilder.DropColumn(
                name: "hectareas",
                schema: "operaciones",
                table: "TB_Cotizaciones");

            migrationBuilder.DropColumn(
                name: "porcentaje_ganancia",
                schema: "operaciones",
                table: "TB_Cotizaciones");

            migrationBuilder.DropColumn(
                name: "porcentaje_riesgo",
                schema: "operaciones",
                table: "TB_Cotizaciones");

            migrationBuilder.DropColumn(
                name: "producto_id",
                schema: "operaciones",
                table: "TB_Cotizaciones");

            migrationBuilder.RenameColumn(
                name: "estado_solicitud",
                schema: "operaciones",
                table: "TB_Cotizaciones",
                newName: "estatus");

            migrationBuilder.AlterColumn<string>(
                name: "telefono",
                schema: "catalogos",
                table: "TB_Proveedor",
                type: "varchar(10)",
                unicode: false,
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(10)",
                oldUnicode: false,
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "nombre",
                schema: "catalogos",
                table: "TB_Proveedor",
                type: "varchar(25)",
                unicode: false,
                maxLength: 25,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(25)",
                oldUnicode: false,
                oldMaxLength: 25);

            migrationBuilder.AlterColumn<int>(
                name: "opinion_id",
                schema: "seguridad",
                table: "TB_Comentarios",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "TB_CotizacionProducto",
                schema: "operaciones",
                columns: table => new
                {
                    id_cotizacion_producto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cotizacion_id = table.Column<int>(type: "int", nullable: false),
                    producto_id = table.Column<int>(type: "int", nullable: false),
                    hectareas = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    porcentaje_ganancia = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    porcentaje_riesgo = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    aplica_riesgo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_CotizacionProducto", x => x.id_cotizacion_producto);
                    table.ForeignKey(
                        name: "FK_CotizacionProducto_Producto",
                        column: x => x.producto_id,
                        principalSchema: "catalogos",
                        principalTable: "TB_Productos",
                        principalColumn: "id_productos");
                    table.ForeignKey(
                        name: "FK_TB_CotizacionProducto_TB_Cotizaciones_cotizacion_id",
                        column: x => x.cotizacion_id,
                        principalSchema: "operaciones",
                        principalTable: "TB_Cotizaciones",
                        principalColumn: "id_cotizaciones",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TB_CotizacionProductoDetalle",
                schema: "operaciones",
                columns: table => new
                {
                    id_cotizacion_producto_detalle = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cotizacion_producto_id = table.Column<int>(type: "int", nullable: false),
                    insumo_id = table.Column<int>(type: "int", nullable: false),
                    cantidad = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    precio_promedio = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_CotizacionProductoDetalle", x => x.id_cotizacion_producto_detalle);
                    table.ForeignKey(
                        name: "FK_CotizacionProductoDetalle_Insumo",
                        column: x => x.insumo_id,
                        principalSchema: "catalogos",
                        principalTable: "TB_Insumo",
                        principalColumn: "id_insumo");
                    table.ForeignKey(
                        name: "FK_TB_CotizacionProductoDetalle_TB_CotizacionProducto_cotizacion_producto_id",
                        column: x => x.cotizacion_producto_id,
                        principalSchema: "operaciones",
                        principalTable: "TB_CotizacionProducto",
                        principalColumn: "id_cotizacion_producto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TB_Cotizaciones_clave_cotizacion",
                schema: "operaciones",
                table: "TB_Cotizaciones",
                column: "clave_cotizacion",
                unique: true,
                filter: "[clave_cotizacion] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TB_CotizacionProducto_cotizacion_id",
                schema: "operaciones",
                table: "TB_CotizacionProducto",
                column: "cotizacion_id");

            migrationBuilder.CreateIndex(
                name: "IX_TB_CotizacionProducto_producto_id",
                schema: "operaciones",
                table: "TB_CotizacionProducto",
                column: "producto_id");

            migrationBuilder.CreateIndex(
                name: "IX_TB_CotizacionProductoDetalle_cotizacion_producto_id",
                schema: "operaciones",
                table: "TB_CotizacionProductoDetalle",
                column: "cotizacion_producto_id");

            migrationBuilder.CreateIndex(
                name: "IX_TB_CotizacionProductoDetalle_insumo_id",
                schema: "operaciones",
                table: "TB_CotizacionProductoDetalle",
                column: "insumo_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_CotizacionProductoDetalle",
                schema: "operaciones");

            migrationBuilder.DropTable(
                name: "TB_CotizacionProducto",
                schema: "operaciones");

            migrationBuilder.DropIndex(
                name: "IX_TB_Cotizaciones_clave_cotizacion",
                schema: "operaciones",
                table: "TB_Cotizaciones");

            migrationBuilder.RenameColumn(
                name: "estatus",
                schema: "operaciones",
                table: "TB_Cotizaciones",
                newName: "estado_solicitud");

            migrationBuilder.AlterColumn<string>(
                name: "telefono",
                schema: "catalogos",
                table: "TB_Proveedor",
                type: "varchar(10)",
                unicode: false,
                maxLength: 10,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(10)",
                oldUnicode: false,
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "nombre",
                schema: "catalogos",
                table: "TB_Proveedor",
                type: "varchar(25)",
                unicode: false,
                maxLength: 25,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(25)",
                oldUnicode: false,
                oldMaxLength: 25,
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "precio_base",
                schema: "catalogos",
                table: "TB_Productos",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "precio_unitario",
                schema: "catalogos",
                table: "TB_Insumo",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "aplica_riesgo",
                schema: "operaciones",
                table: "TB_Cotizaciones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "hectareas",
                schema: "operaciones",
                table: "TB_Cotizaciones",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "porcentaje_ganancia",
                schema: "operaciones",
                table: "TB_Cotizaciones",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "porcentaje_riesgo",
                schema: "operaciones",
                table: "TB_Cotizaciones",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "producto_id",
                schema: "operaciones",
                table: "TB_Cotizaciones",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "opinion_id",
                schema: "seguridad",
                table: "TB_Comentarios",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "TB_CotizacionDetalle",
                schema: "operaciones",
                columns: table => new
                {
                    id_cotizacion_detalle = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cotizacion_id = table.Column<int>(type: "int", nullable: false),
                    insumo_id = table.Column<int>(type: "int", nullable: false),
                    cantidad = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    precio_promedio = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TB_Cotiz__E4E55C7A", x => x.id_cotizacion_detalle);
                    table.ForeignKey(
                        name: "FK_CotizacionDetalle_Cotizacion",
                        column: x => x.cotizacion_id,
                        principalSchema: "operaciones",
                        principalTable: "TB_Cotizaciones",
                        principalColumn: "id_cotizaciones",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CotizacionDetalle_Insumo",
                        column: x => x.insumo_id,
                        principalSchema: "catalogos",
                        principalTable: "TB_Insumo",
                        principalColumn: "id_insumo");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TB_Cotizaciones_producto_id",
                schema: "operaciones",
                table: "TB_Cotizaciones",
                column: "producto_id");

            migrationBuilder.CreateIndex(
                name: "IX_TB_CotizacionDetalle_cotizacion_id",
                schema: "operaciones",
                table: "TB_CotizacionDetalle",
                column: "cotizacion_id");

            migrationBuilder.CreateIndex(
                name: "IX_TB_CotizacionDetalle_insumo_id",
                schema: "operaciones",
                table: "TB_CotizacionDetalle",
                column: "insumo_id");

            migrationBuilder.AddForeignKey(
                name: "FK__TB_Cotiza__produ__06CD04F7",
                schema: "operaciones",
                table: "TB_Cotizaciones",
                column: "producto_id",
                principalSchema: "catalogos",
                principalTable: "TB_Productos",
                principalColumn: "id_productos");
        }
    }
}
