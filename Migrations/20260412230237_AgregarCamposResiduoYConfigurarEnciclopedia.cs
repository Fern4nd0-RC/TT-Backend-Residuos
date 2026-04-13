using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResiduosBackend.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCamposResiduoYConfigurarEnciclopedia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventarios_Perfiles_PerfilId", 
                table: "Inventarios");

            migrationBuilder.DropIndex(
                name: "IX_Inventarios_PerfilId",
                table: "Inventarios");

            migrationBuilder.CreateTable(
                name: "Residuos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Categoria = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Subcategoria = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DescripcionParaNinos = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DatoCurioso = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NombreSprite = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Residuos", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EnciclopediaDesbloqueos",
                columns: table => new
                {
                    PerfilId = table.Column<int>(type: "int", nullable: false),
                    ResiduoId = table.Column<int>(type: "int", nullable: false),
                    FechaDesbloqueo = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnciclopediaDesbloqueos", x => new { x.PerfilId, x.ResiduoId });
                    table.ForeignKey(
                        name: "FK_EnciclopediaDesbloqueos_Perfiles_PerfilId",
                        column: x => x.PerfilId,
                        principalTable: "Perfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EnciclopediaDesbloqueos_Residuos_ResiduoId",
                        column: x => x.ResiduoId,
                        principalTable: "Residuos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Inventarios_PerfilId_ItemId",
                table: "Inventarios",
                columns: new[] { "PerfilId", "ItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EnciclopediaDesbloqueos_ResiduoId",
                table: "EnciclopediaDesbloqueos",
                column: "ResiduoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventarios_Perfiles_PerfilId",
                table: "Inventarios",
                column: "PerfilId",
                principalTable: "Perfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EnciclopediaDesbloqueos");

            migrationBuilder.DropTable(
                name: "Residuos");

            migrationBuilder.DropIndex(
                name: "IX_Inventarios_PerfilId_ItemId",
                table: "Inventarios");

            migrationBuilder.CreateIndex(
                name: "IX_Inventarios_PerfilId",
                table: "Inventarios",
                column: "PerfilId");
        }
    }
}
