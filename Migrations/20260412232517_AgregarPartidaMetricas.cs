using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResiduosBackend.Migrations
{
    /// <inheritdoc />
    public partial class AgregarPartidaMetricas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PartidaMetricas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PerfilId = table.Column<int>(type: "int", nullable: false),
                    PuntuacionObtenida = table.Column<int>(type: "int", nullable: false),
                    ResiduosClasificadosCorrectamente = table.Column<int>(type: "int", nullable: false),
                    FechaPartida = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartidaMetricas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartidaMetricas_Perfiles_PerfilId",
                        column: x => x.PerfilId,
                        principalTable: "Perfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_PartidaMetricas_PerfilId",
                table: "PartidaMetricas",
                column: "PerfilId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PartidaMetricas");
        }
    }
}
