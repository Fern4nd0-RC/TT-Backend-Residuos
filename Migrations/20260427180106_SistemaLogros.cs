using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ResiduosBackend.Migrations
{
    /// <inheritdoc />
    public partial class SistemaLogros : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ResiduosInorganicosClasificados",
                table: "PartidaMetricas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ResiduosOrganicosClasificados",
                table: "PartidaMetricas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Logros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descripcion = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ImagenUrl = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RequisitoXP = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logros", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PerfilLogros",
                columns: table => new
                {
                    PerfilId = table.Column<int>(type: "int", nullable: false),
                    LogroId = table.Column<int>(type: "int", nullable: false),
                    FechaObtencion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfilLogros", x => new { x.PerfilId, x.LogroId });
                    table.ForeignKey(
                        name: "FK_PerfilLogros_Logros_LogroId",
                        column: x => x.LogroId,
                        principalTable: "Logros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PerfilLogros_Perfiles_PerfilId",
                        column: x => x.PerfilId,
                        principalTable: "Perfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Logros",
                columns: new[] { "Id", "Descripcion", "ImagenUrl", "Nombre", "RequisitoXP" },
                values: new object[,]
                {
                    { 1, "Alcanza 100 XP acumulada.", "logros/reciclador-novato.png", "Reciclador Novato", 100 },
                    { 2, "Alcanza 500 XP acumulada.", "logros/guardian-planeta.png", "Guardian del Planeta", 500 },
                    { 3, "Alcanza 1000 XP acumulada.", "logros/maestro-reciclaje.png", "Maestro del Reciclaje", 1000 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PerfilLogros_LogroId",
                table: "PerfilLogros",
                column: "LogroId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PerfilLogros");

            migrationBuilder.DropTable(
                name: "Logros");

            migrationBuilder.DropColumn(
                name: "ResiduosInorganicosClasificados",
                table: "PartidaMetricas");

            migrationBuilder.DropColumn(
                name: "ResiduosOrganicosClasificados",
                table: "PartidaMetricas");
        }
    }
}
