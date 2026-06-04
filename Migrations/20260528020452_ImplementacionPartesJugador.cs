using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ResiduosBackend.Migrations
{
    /// <inheritdoc />
    public partial class ImplementacionPartesJugador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColorHex",
                table: "Perfiles");

            migrationBuilder.RenameColumn(
                name: "IndiceImagen",
                table: "Perfiles",
                newName: "HatPartId");

            migrationBuilder.AddColumn<int>(
                name: "BodyPartId",
                table: "Perfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FacePartId",
                table: "Perfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AvatarParts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Slot = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nombre = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RecursoUnity = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Orden = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvatarParts", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "AvatarParts",
                columns: new[] { "Id", "Nombre", "Orden", "RecursoUnity", "Slot" },
                values: new object[,]
                {
                    { 101, "Azul Base", 0, "Blue 2 Base", "Body" },
                    { 102, "Azul Oscuro", 1, "Blue 1 Dark", "Body" },
                    { 103, "Azul Claro", 2, "Blue 3 Light", "Body" },
                    { 104, "Rojo Base", 3, "Red 2 Base", "Body" },
                    { 105, "Rojo Oscuro", 4, "Red 1 Dark", "Body" },
                    { 106, "Rojo Claro", 5, "Red 3 Light", "Body" },
                    { 107, "Verde Base", 6, "Green 2 Base", "Body" },
                    { 108, "Amarillo Base", 7, "Yellow 2 Base", "Body" },
                    { 109, "Morado Base", 8, "Purple 2 Base", "Body" },
                    { 110, "Rosa Base", 9, "Pink 2 Base", "Body" },
                    { 111, "Naranja Base", 10, "Orange 2 Base", "Body" },
                    { 112, "Cian Base", 11, "Cyan 2 Base", "Body" },
                    { 113, "Turquesa Base", 12, "Turquoise 2 Base", "Body" },
                    { 114, "Café Base", 13, "Brown 2 Base", "Body" },
                    { 115, "Crema Base", 14, "Cream 2 Base", "Body" },
                    { 116, "Gris Base", 15, "Grey 2 Base", "Body" },
                    { 201, "Feliz", 0, "face 1", "Face" },
                    { 202, "Enojado", 1, "face 2", "Face" },
                    { 203, "Triste", 2, "face 3", "Face" },
                    { 301, "Sin sombrero", 0, "", "Hat" },
                    { 302, "Sombrero de chef", 1, "chef hat", "Hat" },
                    { 303, "Sombrero de fiesta", 2, "party hat", "Hat" },
                    { 304, "Sombrero naranja", 3, "orange hat", "Hat" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Perfiles_BodyPartId",
                table: "Perfiles",
                column: "BodyPartId");

            migrationBuilder.CreateIndex(
                name: "IX_Perfiles_FacePartId",
                table: "Perfiles",
                column: "FacePartId");

            migrationBuilder.CreateIndex(
                name: "IX_Perfiles_HatPartId",
                table: "Perfiles",
                column: "HatPartId");

            migrationBuilder.CreateIndex(
                name: "IX_AvatarParts_Slot_Orden",
                table: "AvatarParts",
                columns: new[] { "Slot", "Orden" });

            migrationBuilder.AddForeignKey(
                name: "FK_Perfiles_AvatarParts_BodyPartId",
                table: "Perfiles",
                column: "BodyPartId",
                principalTable: "AvatarParts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Perfiles_AvatarParts_FacePartId",
                table: "Perfiles",
                column: "FacePartId",
                principalTable: "AvatarParts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Perfiles_AvatarParts_HatPartId",
                table: "Perfiles",
                column: "HatPartId",
                principalTable: "AvatarParts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Perfiles_AvatarParts_BodyPartId",
                table: "Perfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Perfiles_AvatarParts_FacePartId",
                table: "Perfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Perfiles_AvatarParts_HatPartId",
                table: "Perfiles");

            migrationBuilder.DropTable(
                name: "AvatarParts");

            migrationBuilder.DropIndex(
                name: "IX_Perfiles_BodyPartId",
                table: "Perfiles");

            migrationBuilder.DropIndex(
                name: "IX_Perfiles_FacePartId",
                table: "Perfiles");

            migrationBuilder.DropIndex(
                name: "IX_Perfiles_HatPartId",
                table: "Perfiles");

            migrationBuilder.DropColumn(
                name: "BodyPartId",
                table: "Perfiles");

            migrationBuilder.DropColumn(
                name: "FacePartId",
                table: "Perfiles");

            migrationBuilder.RenameColumn(
                name: "HatPartId",
                table: "Perfiles",
                newName: "IndiceImagen");

            migrationBuilder.AddColumn<string>(
                name: "ColorHex",
                table: "Perfiles",
                type: "varchar(7)",
                maxLength: 7,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
