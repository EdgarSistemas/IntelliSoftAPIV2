using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntelliSoftAPIV2.Migrations
{
    /// <inheritdoc />
    public partial class AddPorcentajes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "porcentaje_ganancia",
                schema: "catalogos",
                table: "TB_Productos",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "porcentaje_riesgo",
                schema: "catalogos",
                table: "TB_Productos",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "aplica_riesgo",
                schema: "operaciones",
                table: "TB_Cotizaciones",
                type: "int",
                nullable: false,
                defaultValue: 0);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "porcentaje_ganancia",
                schema: "catalogos",
                table: "TB_Productos");

            migrationBuilder.DropColumn(
                name: "porcentaje_riesgo",
                schema: "catalogos",
                table: "TB_Productos");

            migrationBuilder.DropColumn(
                name: "aplica_riesgo",
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
        }
    }
}
