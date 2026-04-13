using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResiduosBackend.Migrations
{
    /// <inheritdoc />
    public partial class AgregarNivelYEstrellas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EstrellaSostenibilidad",
                table: "Perfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Nivel",
                table: "Perfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstrellaSostenibilidad",
                table: "Perfiles");

            migrationBuilder.DropColumn(
                name: "Nivel",
                table: "Perfiles");
        }
    }
}
