using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntelliSoftAPIV2.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnClaveCoti : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "clave_cotizacion",
                schema: "operaciones",
                table: "TB_Cotizaciones",
                type: "nvarchar(65)",
                maxLength: 65,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "clave_cotizacion",
                schema: "operaciones",
                table: "TB_Cotizaciones");
        }
    }
}
