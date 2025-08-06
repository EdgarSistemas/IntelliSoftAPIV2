using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntelliSoftAPIV2.Migrations
{
    /// <inheritdoc />
    public partial class AddRelacionOpiCom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__TB_Coment__usuar__0C85DE4D",
                schema: "seguridad",
                table: "TB_Comentarios");

            migrationBuilder.DropIndex(
                name: "IX_TB_Comentarios_usuario_id",
                schema: "seguridad",
                table: "TB_Comentarios");

            migrationBuilder.DropColumn(
                name: "usuario_id",
                schema: "seguridad",
                table: "TB_Comentarios");

            migrationBuilder.AddColumn<int>(
                name: "opinion_id",
                schema: "seguridad",
                table: "TB_Comentarios",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TB_Comentarios_opinion_id",
                schema: "seguridad",
                table: "TB_Comentarios",
                column: "opinion_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comentario_Opinion",
                schema: "seguridad",
                table: "TB_Comentarios",
                column: "opinion_id",
                principalSchema: "operaciones",
                principalTable: "TB_Opiniones",
                principalColumn: "id_opinion");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comentario_Opinion",
                schema: "seguridad",
                table: "TB_Comentarios");

            migrationBuilder.DropIndex(
                name: "IX_TB_Comentarios_opinion_id",
                schema: "seguridad",
                table: "TB_Comentarios");

            migrationBuilder.DropColumn(
                name: "opinion_id",
                schema: "seguridad",
                table: "TB_Comentarios");

            migrationBuilder.AddColumn<string>(
                name: "usuario_id",
                schema: "seguridad",
                table: "TB_Comentarios",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TB_Comentarios_usuario_id",
                schema: "seguridad",
                table: "TB_Comentarios",
                column: "usuario_id");

            migrationBuilder.AddForeignKey(
                name: "FK__TB_Coment__usuar__0C85DE4D",
                schema: "seguridad",
                table: "TB_Comentarios",
                column: "usuario_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
