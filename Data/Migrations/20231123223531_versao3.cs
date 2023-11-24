using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ficha1_P1_V1.Data.Migrations
{
    public partial class versao3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataFim",
                table: "Habitacao");

            migrationBuilder.DropColumn(
                name: "DataInicio",
                table: "Habitacao");

            migrationBuilder.DropColumn(
                name: "Locador",
                table: "Habitacao");

            migrationBuilder.DropColumn(
                name: "LocadorAvaliacao",
                table: "Habitacao");

            migrationBuilder.DropColumn(
                name: "PeriodoMaximo",
                table: "Habitacao");

            migrationBuilder.DropColumn(
                name: "PeriodoMinimo",
                table: "Habitacao");

            migrationBuilder.DropColumn(
                name: "Preco",
                table: "Habitacao");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataFim",
                table: "Habitacao",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataInicio",
                table: "Habitacao",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Locador",
                table: "Habitacao",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "LocadorAvaliacao",
                table: "Habitacao",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PeriodoMaximo",
                table: "Habitacao",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PeriodoMinimo",
                table: "Habitacao",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Preco",
                table: "Habitacao",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
