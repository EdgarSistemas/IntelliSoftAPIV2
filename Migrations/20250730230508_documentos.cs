using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntelliSoftAPIV2.Migrations
{
    /// <inheritdoc />
    public partial class documentos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_Documento",
                schema: "catalogos",
                columns: table => new
                {
                    IdDocumento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_producto = table.Column<int>(type: "int", nullable: false),
                    nombre_columna = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductoIdProductos = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_Documento_IdDocumento", x => x.IdDocumento);
                    table.ForeignKey(
                        name: "FK_TB_Documento_TB_Productos_ProductoIdProductos",
                        column: x => x.ProductoIdProductos,
                        principalSchema: "catalogos",
                        principalTable: "TB_Productos",
                        principalColumn: "id_productos");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TB_Documento_ProductoIdProductos",
                schema: "catalogos",
                table: "TB_Documento",
                column: "ProductoIdProductos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_Documento",
                schema: "catalogos");
        }
    }
}
