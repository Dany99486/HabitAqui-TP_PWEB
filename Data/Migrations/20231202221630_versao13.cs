using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ficha1_P1_V1.Data.Migrations
{
    public partial class versao13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quartos",
                table: "Habitacao");

            migrationBuilder.AddColumn<int>(
                name: "CategoriaId",
                table: "Habitacao",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Categoria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Disponivel = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categoria", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Habitacao_CategoriaId",
                table: "Habitacao",
                column: "CategoriaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Habitacao_Categoria_CategoriaId",
                table: "Habitacao",
                column: "CategoriaId",
                principalTable: "Categoria",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Habitacao_Categoria_CategoriaId",
                table: "Habitacao");

            migrationBuilder.DropTable(
                name: "Categoria");

            migrationBuilder.DropIndex(
                name: "IX_Habitacao_CategoriaId",
                table: "Habitacao");

            migrationBuilder.DropColumn(
                name: "CategoriaId",
                table: "Habitacao");

            migrationBuilder.AddColumn<int>(
                name: "Quartos",
                table: "Habitacao",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
