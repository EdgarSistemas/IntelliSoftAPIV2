using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntelliSoftAPIV2.Migrations
{
    /// <inheritdoc />
    public partial class RefactorPedidoCotizacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__TB_Pedido__clien__05D8E0BE",
                schema: "operaciones",
                table: "TB_Pedido");

            migrationBuilder.DropTable(
                name: "TB_PedidoDetalle",
                schema: "operaciones");

            migrationBuilder.DropColumn(
                name: "total",
                schema: "operaciones",
                table: "TB_Pedido");

            migrationBuilder.RenameColumn(
                name: "cliente_id",
                schema: "operaciones",
                table: "TB_Pedido",
                newName: "ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_TB_Pedido_cliente_id",
                schema: "operaciones",
                table: "TB_Pedido",
                newName: "IX_TB_Pedido_ApplicationUserId");

            migrationBuilder.AddColumn<int>(
                name: "cantidad",
                schema: "operaciones",
                table: "TB_Pedido",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "cotizacion_id",
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
                name: "nombre",
                schema: "catalogos",
                table: "TB_CatalogoUnidad",
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

            migrationBuilder.CreateIndex(
                name: "IX_TB_Pedido_cotizacion_id",
                schema: "operaciones",
                table: "TB_Pedido",
                column: "cotizacion_id");

            migrationBuilder.AddForeignKey(
                name: "FK_TB_Pedido_AspNetUsers_ApplicationUserId",
                schema: "operaciones",
                table: "TB_Pedido",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TB_Pedido_Cotizacion",
                schema: "operaciones",
                table: "TB_Pedido",
                column: "cotizacion_id",
                principalSchema: "operaciones",
                principalTable: "TB_Cotizaciones",
                principalColumn: "id_cotizaciones");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TB_Pedido_AspNetUsers_ApplicationUserId",
                schema: "operaciones",
                table: "TB_Pedido");

            migrationBuilder.DropForeignKey(
                name: "FK_TB_Pedido_Cotizacion",
                schema: "operaciones",
                table: "TB_Pedido");

            migrationBuilder.DropIndex(
                name: "IX_TB_Pedido_cotizacion_id",
                schema: "operaciones",
                table: "TB_Pedido");

            migrationBuilder.DropColumn(
                name: "cantidad",
                schema: "operaciones",
                table: "TB_Pedido");

            migrationBuilder.DropColumn(
                name: "cotizacion_id",
                schema: "operaciones",
                table: "TB_Pedido");

            migrationBuilder.DropColumn(
                name: "precio_unitario",
                schema: "operaciones",
                table: "TB_Pedido");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                schema: "operaciones",
                table: "TB_Pedido",
                newName: "cliente_id");

            migrationBuilder.RenameIndex(
                name: "IX_TB_Pedido_ApplicationUserId",
                schema: "operaciones",
                table: "TB_Pedido",
                newName: "IX_TB_Pedido_cliente_id");

            migrationBuilder.AddColumn<decimal>(
                name: "total",
                schema: "operaciones",
                table: "TB_Pedido",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "nombre",
                schema: "catalogos",
                table: "TB_CatalogoUnidad",
                type: "varchar(25)",
                unicode: false,
                maxLength: 25,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(25)",
                oldUnicode: false,
                oldMaxLength: 25);

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

            migrationBuilder.AddForeignKey(
                name: "FK__TB_Pedido__clien__05D8E0BE",
                schema: "operaciones",
                table: "TB_Pedido",
                column: "cliente_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
