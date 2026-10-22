using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoFinalG5.Migrations
{
    /// <inheritdoc />
    public partial class m4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Servicios_IdServicio",
                table: "Reservas");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservaServicios_Reservas_ReservaIdReserva",
                table: "ReservaServicios");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservaServicios_Servicios_ServicioIdServicio",
                table: "ReservaServicios");

            migrationBuilder.DropIndex(
                name: "IX_ReservaServicios_ReservaIdReserva",
                table: "ReservaServicios");

            migrationBuilder.DropIndex(
                name: "IX_ReservaServicios_ServicioIdServicio",
                table: "ReservaServicios");

            migrationBuilder.DropIndex(
                name: "IX_Reservas_IdServicio",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "ReservaIdReserva",
                table: "ReservaServicios");

            migrationBuilder.DropColumn(
                name: "ServicioIdServicio",
                table: "ReservaServicios");

            migrationBuilder.AddColumn<int>(
                name: "ServicioIdServicio",
                table: "Reservas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ReservaServicios_IdReserva",
                table: "ReservaServicios",
                column: "IdReserva");

            migrationBuilder.CreateIndex(
                name: "IX_ReservaServicios_IdServicio",
                table: "ReservaServicios",
                column: "IdServicio");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_ServicioIdServicio",
                table: "Reservas",
                column: "ServicioIdServicio");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Servicios_ServicioIdServicio",
                table: "Reservas",
                column: "ServicioIdServicio",
                principalTable: "Servicios",
                principalColumn: "IdServicio",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservaServicios_Reservas_IdReserva",
                table: "ReservaServicios",
                column: "IdReserva",
                principalTable: "Reservas",
                principalColumn: "IdReserva",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservaServicios_Servicios_IdServicio",
                table: "ReservaServicios",
                column: "IdServicio",
                principalTable: "Servicios",
                principalColumn: "IdServicio",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Servicios_ServicioIdServicio",
                table: "Reservas");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservaServicios_Reservas_IdReserva",
                table: "ReservaServicios");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservaServicios_Servicios_IdServicio",
                table: "ReservaServicios");

            migrationBuilder.DropIndex(
                name: "IX_ReservaServicios_IdReserva",
                table: "ReservaServicios");

            migrationBuilder.DropIndex(
                name: "IX_ReservaServicios_IdServicio",
                table: "ReservaServicios");

            migrationBuilder.DropIndex(
                name: "IX_Reservas_ServicioIdServicio",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "ServicioIdServicio",
                table: "Reservas");

            migrationBuilder.AddColumn<int>(
                name: "ReservaIdReserva",
                table: "ReservaServicios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ServicioIdServicio",
                table: "ReservaServicios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ReservaServicios_ReservaIdReserva",
                table: "ReservaServicios",
                column: "ReservaIdReserva");

            migrationBuilder.CreateIndex(
                name: "IX_ReservaServicios_ServicioIdServicio",
                table: "ReservaServicios",
                column: "ServicioIdServicio");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_IdServicio",
                table: "Reservas",
                column: "IdServicio");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Servicios_IdServicio",
                table: "Reservas",
                column: "IdServicio",
                principalTable: "Servicios",
                principalColumn: "IdServicio",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservaServicios_Reservas_ReservaIdReserva",
                table: "ReservaServicios",
                column: "ReservaIdReserva",
                principalTable: "Reservas",
                principalColumn: "IdReserva",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservaServicios_Servicios_ServicioIdServicio",
                table: "ReservaServicios",
                column: "ServicioIdServicio",
                principalTable: "Servicios",
                principalColumn: "IdServicio",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
