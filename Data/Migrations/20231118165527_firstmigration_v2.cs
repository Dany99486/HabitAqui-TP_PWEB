using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ficha1_P1_V1.Data.Migrations
{
    public partial class firstmigration_v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataFim",
                table: "Habitacao",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DataInicio",
                table: "Habitacao",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

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
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Localizacao",
                table: "Habitacao",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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

            migrationBuilder.AddColumn<string>(
                name: "Tipo",
                table: "Habitacao",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "Localizacao",
                table: "Habitacao");

            migrationBuilder.DropColumn(
                name: "PeriodoMinimo",
                table: "Habitacao");

            migrationBuilder.DropColumn(
                name: "Preco",
                table: "Habitacao");

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "Habitacao");
        }
    }
}
