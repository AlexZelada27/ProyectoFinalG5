using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoFinalG5.Migrations
{
    /// <inheritdoc />
    public partial class m5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Servicios_ServicioIdServicio",
                table: "Reservas");

            migrationBuilder.DropIndex(
                name: "IX_Reservas_ServicioIdServicio",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "IdServicio",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "ServicioIdServicio",
                table: "Reservas");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdServicio",
                table: "Reservas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ServicioIdServicio",
                table: "Reservas",
                type: "int",
                nullable: false,
                defaultValue: 0);

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
        }
    }
}
