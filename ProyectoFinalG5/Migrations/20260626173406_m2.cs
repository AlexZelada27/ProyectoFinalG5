using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoFinalG5.Migrations
{
    /// <inheritdoc />
    public partial class m2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Horarios_IdHorario",
                table: "Reservas");

            migrationBuilder.DropTable(
                name: "Horarios");

            migrationBuilder.RenameColumn(
                name: "IdHorario",
                table: "Reservas",
                newName: "IdHorarioFijo");

            migrationBuilder.RenameIndex(
                name: "IX_Reservas_IdHorario",
                table: "Reservas",
                newName: "IX_Reservas_IdHorarioFijo");

            migrationBuilder.CreateTable(
                name: "HorariosFijos",
                columns: table => new
                {
                    IdHorarioFijo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoraInicio = table.Column<TimeSpan>(type: "time", nullable: false),
                    HoraFin = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HorariosFijos", x => x.IdHorarioFijo);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_HorariosFijos_IdHorarioFijo",
                table: "Reservas",
                column: "IdHorarioFijo",
                principalTable: "HorariosFijos",
                principalColumn: "IdHorarioFijo",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_HorariosFijos_IdHorarioFijo",
                table: "Reservas");

            migrationBuilder.DropTable(
                name: "HorariosFijos");

            migrationBuilder.RenameColumn(
                name: "IdHorarioFijo",
                table: "Reservas",
                newName: "IdHorario");

            migrationBuilder.RenameIndex(
                name: "IX_Reservas_IdHorarioFijo",
                table: "Reservas",
                newName: "IX_Reservas_IdHorario");

            migrationBuilder.CreateTable(
                name: "Horarios",
                columns: table => new
                {
                    IdHorario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Hora = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Horarios", x => x.IdHorario);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Horarios_IdHorario",
                table: "Reservas",
                column: "IdHorario",
                principalTable: "Horarios",
                principalColumn: "IdHorario",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
