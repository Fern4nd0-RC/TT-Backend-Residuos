using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResiduosBackend.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCamposTiendaEnItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CostoEstrellas",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CostoFichas",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NivelDesbloqueo",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CostoEstrellas",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "CostoFichas",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "NivelDesbloqueo",
                table: "Items");
        }
    }
}
