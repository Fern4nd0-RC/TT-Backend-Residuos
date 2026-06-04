using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResiduosBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddCostoFichasResiduo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CostoFichas",
                table: "Residuos",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CostoFichas",
                table: "Residuos");
        }
    }
}
