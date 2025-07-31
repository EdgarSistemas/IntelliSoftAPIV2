using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntelliSoftAPIV2.Migrations
{
    /// <inheritdoc />
    public partial class AddEstatusProducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "estatus",
                schema: "catalogos",
                table: "TB_Productos",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "estatus",
                schema: "catalogos",
                table: "TB_Productos");
        }
    }
}
