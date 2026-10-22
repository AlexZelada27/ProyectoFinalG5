using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoFinalG5.Migrations
{
    /// <inheritdoc />
    public partial class m10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HorarioIdHorario",
                table: "Reservas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdHorario",
                table: "Reservas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Horario",
                columns: table => new
                {
                    IdHorario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraInicio = table.Column<TimeSpan>(type: "time", nullable: false),
                    HoraFin = table.Column<TimeSpan>(type: "time", nullable: false),
                    Reservado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Horario", x => x.IdHorario);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_HorarioIdHorario",
                table: "Reservas",
                column: "HorarioIdHorario");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Horario_HorarioIdHorario",
                table: "Reservas",
                column: "HorarioIdHorario",
                principalTable: "Horario",
                principalColumn: "IdHorario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Horario_HorarioIdHorario",
                table: "Reservas");

            migrationBuilder.DropTable(
                name: "Horario");

            migrationBuilder.DropIndex(
                name: "IX_Reservas_HorarioIdHorario",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "HorarioIdHorario",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "IdHorario",
                table: "Reservas");
        }
    }
}
