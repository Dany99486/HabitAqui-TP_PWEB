using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ficha1_P1_V1.Data.Migrations
{
    public partial class versao19 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Habitacao_empresaId",
                table: "Habitacao",
                column: "empresaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Habitacao_Empresa_empresaId",
                table: "Habitacao",
                column: "empresaId",
                principalTable: "Empresa",
                principalColumn: "EmpresaId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Habitacao_Empresa_empresaId",
                table: "Habitacao");

            migrationBuilder.DropIndex(
                name: "IX_Habitacao_empresaId",
                table: "Habitacao");
        }
    }
}
