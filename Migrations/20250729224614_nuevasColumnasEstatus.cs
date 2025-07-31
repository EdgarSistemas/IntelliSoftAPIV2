using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntelliSoftAPIV2.Migrations
{
    /// <inheritdoc />
    public partial class nuevasColumnasEstatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "estatus",
                schema: "operaciones",
                table: "TB_Opiniones",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "estatus",
                schema: "seguridad",
                table: "TB_Comentarios",
                type: "int",
                nullable: false,
                defaultValue: 1);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "estatus",
                schema: "operaciones",
                table: "TB_Opiniones");

            migrationBuilder.DropColumn(
                name: "estatus",
                schema: "seguridad",
                table: "TB_Comentarios");

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
        }
    }
}
