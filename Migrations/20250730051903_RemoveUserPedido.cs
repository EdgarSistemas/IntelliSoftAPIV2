using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntelliSoftAPIV2.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserPedido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TB_Pedido_AspNetUsers_ApplicationUserId",
                table: "TB_Pedido",
                schema: "operaciones");

            migrationBuilder.DropIndex(
                name: "IX_TB_Pedido_ApplicationUserId",
                table: "TB_Pedido",
                schema: "operaciones");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "TB_Pedido",
                schema: "operaciones");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "TB_Pedido",
                schema: "operaciones",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TB_Pedido_ApplicationUserId",
                table: "TB_Pedido",
                schema: "operaciones",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TB_Pedido_AspNetUsers_ApplicationUserId",
                table: "TB_Pedido",
                schema: "operaciones",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
