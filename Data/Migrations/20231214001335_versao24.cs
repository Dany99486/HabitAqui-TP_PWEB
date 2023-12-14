using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ficha1_P1_V1.Data.Migrations
{
    public partial class versao24 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "EstadoHabitacao",
                table: "Habitacao",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "QuererReserva",
                table: "Habitacao",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ReservadoClienteId",
                table: "Habitacao",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DanosHabitacao",
                table: "Arrendamento",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DanosHabitacaoC",
                table: "Arrendamento",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EquipamentosOpcionais",
                table: "Arrendamento",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EquipamentosOpcionaisC",
                table: "Arrendamento",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EstadoEntregue",
                table: "Arrendamento",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EstadoRecebido",
                table: "Arrendamento",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Observacoes",
                table: "Arrendamento",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObservacoesC",
                table: "Arrendamento",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Habitacao_ReservadoClienteId",
                table: "Habitacao",
                column: "ReservadoClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Habitacao_AspNetUsers_ReservadoClienteId",
                table: "Habitacao",
                column: "ReservadoClienteId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Habitacao_AspNetUsers_ReservadoClienteId",
                table: "Habitacao");

            migrationBuilder.DropIndex(
                name: "IX_Habitacao_ReservadoClienteId",
                table: "Habitacao");

            migrationBuilder.DropColumn(
                name: "QuererReserva",
                table: "Habitacao");

            migrationBuilder.DropColumn(
                name: "ReservadoClienteId",
                table: "Habitacao");

            migrationBuilder.DropColumn(
                name: "DanosHabitacao",
                table: "Arrendamento");

            migrationBuilder.DropColumn(
                name: "DanosHabitacaoC",
                table: "Arrendamento");

            migrationBuilder.DropColumn(
                name: "EquipamentosOpcionais",
                table: "Arrendamento");

            migrationBuilder.DropColumn(
                name: "EquipamentosOpcionaisC",
                table: "Arrendamento");

            migrationBuilder.DropColumn(
                name: "EstadoEntregue",
                table: "Arrendamento");

            migrationBuilder.DropColumn(
                name: "EstadoRecebido",
                table: "Arrendamento");

            migrationBuilder.DropColumn(
                name: "Observacoes",
                table: "Arrendamento");

            migrationBuilder.DropColumn(
                name: "ObservacoesC",
                table: "Arrendamento");

            migrationBuilder.AlterColumn<string>(
                name: "EstadoHabitacao",
                table: "Habitacao",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
