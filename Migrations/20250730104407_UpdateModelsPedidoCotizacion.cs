using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntelliSoftAPIV2.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelsPedidoCotizacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cantidad",
                schema: "operaciones",
                table: "TB_Pedido");

            migrationBuilder.DropColumn(
                name: "precio_unitario",
                schema: "operaciones",
                table: "TB_Pedido");

            migrationBuilder.DropColumn(
                name: "email_solicitante",
                schema: "operaciones",
                table: "TB_Cotizaciones");

            migrationBuilder.DropColumn(
                name: "nombre_solicitante",
                schema: "operaciones",
                table: "TB_Cotizaciones");

            migrationBuilder.AlterColumn<int>(
                name: "estatus",
                schema: "operaciones",
                table: "TB_Pedido",
                type: "int",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "estado_solicitud",
                schema: "operaciones",
                table: "TB_Cotizaciones",
                type: "int",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "hectareas",
                schema: "operaciones",
                table: "TB_Cotizaciones",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

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
                name: "IX_TB_CotizacionDetalle_cotizacion_id",
                schema: "operaciones",
                table: "TB_CotizacionDetalle",
                column: "cotizacion_id");

            migrationBuilder.CreateIndex(
                name: "IX_TB_CotizacionDetalle_insumo_id",
                schema: "operaciones",
                table: "TB_CotizacionDetalle",
                column: "insumo_id");

            migrationBuilder.CreateIndex(
                name: "IX_TB_InventarioInsumo_insumo_id",
                schema: "almacen",
                table: "TB_InventarioInsumo",
                column: "insumo_id");

            migrationBuilder.CreateIndex(
                name: "IX_TB_InventarioInsumo_compra_id",
                schema: "almacen",
                table: "TB_InventarioInsumo",
                column: "compra_id");

            migrationBuilder.CreateIndex(
                name: "IX_TB_InventarioInsumo_pedido_id",
                schema: "almacen",
                table: "TB_InventarioInsumo",
                column: "pedido_id");

            migrationBuilder.AddForeignKey(
                name: "FK__TB_Invent__insum__02084FDA",
                schema: "almacen",
                table: "TB_InventarioInsumo",
                column: "insumo_id",
                principalSchema: "catalogos",
                principalTable: "TB_Insumo",
                principalColumn: "id_insumo",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK__TB_Invent__compr__01142BA1",
                schema: "almacen",
                table: "TB_InventarioInsumo",
                column: "compra_id",
                principalSchema: "almacen",
                principalTable: "TB_Compra",
                principalColumn: "id_compra",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK__TB_Invent__pedid__00200768",
                schema: "almacen",
                table: "TB_InventarioInsumo",
                column: "pedido_id",
                principalSchema: "operaciones",
                principalTable: "TB_Pedido",
                principalColumn: "id_pedido",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__TB_Invent__insum__02084FDA",
                schema: "almacen",
                table: "TB_InventarioInsumo");

            migrationBuilder.DropForeignKey(
                name: "FK__TB_Invent__compr__01142BA1",
                schema: "almacen",
                table: "TB_InventarioInsumo");

            migrationBuilder.DropForeignKey(
                name: "FK__TB_Invent__pedid__00200768",
                schema: "almacen",
                table: "TB_InventarioInsumo");

            migrationBuilder.DropIndex(
                name: "IX_TB_InventarioInsumo_insumo_id",
                schema: "almacen",
                table: "TB_InventarioInsumo");

            migrationBuilder.DropIndex(
                name: "IX_TB_InventarioInsumo_compra_id",
                schema: "almacen",
                table: "TB_InventarioInsumo");

            migrationBuilder.DropIndex(
                name: "IX_TB_InventarioInsumo_pedido_id",
                schema: "almacen",
                table: "TB_InventarioInsumo");

            migrationBuilder.DropTable(
                name: "TB_CotizacionDetalle",
                schema: "operaciones");

            migrationBuilder.DropColumn(
                name: "hectareas",
                schema: "operaciones",
                table: "TB_Cotizaciones");

            migrationBuilder.AlterColumn<int>(
                name: "estatus",
                schema: "operaciones",
                table: "TB_Pedido",
                type: "int",
                unicode: false,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "cantidad",
                schema: "operaciones",
                table: "TB_Pedido",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "precio_unitario",
                schema: "operaciones",
                table: "TB_Pedido",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "estado_solicitud",
                schema: "operaciones",
                table: "TB_Cotizaciones",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "email_solicitante",
                schema: "operaciones",
                table: "TB_Cotizaciones",
                type: "varchar(255)",
                unicode: false,
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "nombre_solicitante",
                schema: "operaciones",
                table: "TB_Cotizaciones",
                type: "varchar(150)",
                unicode: false,
                maxLength: 150,
                nullable: true);
        }
    }
}
