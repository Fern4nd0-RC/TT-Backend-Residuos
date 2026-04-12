using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResiduosBackend.Migrations
{
    /// <inheritdoc />
    public partial class AgregarItemsEInventario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EnciclopediaDesbloqueos");

            migrationBuilder.DropTable(
                name: "PartidasMetricas");

            migrationBuilder.DropTable(
                name: "Residuos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Inventarios",
                table: "Inventarios");

            migrationBuilder.DropColumn(
                name: "Costo",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "FechaCompra",
                table: "Inventarios");

            migrationBuilder.AlterColumn<string>(
                name: "Tipo",
                table: "Items",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Items",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "Items",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NombreSprite",
                table: "Items",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Inventarios",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Cantidad",
                table: "Inventarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Inventarios",
                table: "Inventarios",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Inventarios_PerfilId",
                table: "Inventarios",
                column: "PerfilId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Inventarios",
                table: "Inventarios");

            migrationBuilder.DropIndex(
                name: "IX_Inventarios_PerfilId",
                table: "Inventarios");

            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "NombreSprite",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Inventarios");

            migrationBuilder.DropColumn(
                name: "Cantidad",
                table: "Inventarios");

            migrationBuilder.AlterColumn<string>(
                name: "Tipo",
                table: "Items",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Items",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<int>(
                name: "Costo",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCompra",
                table: "Inventarios",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Inventarios",
                table: "Inventarios",
                columns: new[] { "PerfilId", "ItemId" });

            migrationBuilder.CreateTable(
                name: "PartidasMetricas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PerfilId = table.Column<int>(type: "int", nullable: false),
                    FechaPartida = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PuntuacionObtenida = table.Column<int>(type: "int", nullable: false),
                    ResiduosClasificadosCorrectamente = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartidasMetricas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartidasMetricas_Perfiles_PerfilId",
                        column: x => x.PerfilId,
                        principalTable: "Perfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Residuos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Categoria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Residuos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EnciclopediaDesbloqueos",
                columns: table => new
                {
                    PerfilId = table.Column<int>(type: "int", nullable: false),
                    ResiduoId = table.Column<int>(type: "int", nullable: false),
                    FechaDesbloqueo = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EnciclopediaDesbloqueos_ResiduoId",
                table: "EnciclopediaDesbloqueos",
                column: "ResiduoId");

            migrationBuilder.CreateIndex(
                name: "IX_PartidasMetricas_PerfilId",
                table: "PartidasMetricas",
                column: "PerfilId");
        }
    }
}
