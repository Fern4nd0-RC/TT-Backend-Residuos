using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ResiduosBackend.Migrations
{
    /// <inheritdoc />
    public partial class ImplementacionFotoPerfil : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProfilePictureId",
                table: "Perfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AvatarParts",
                keyColumn: "Id",
                keyValue: 304,
                column: "RecursoUnity",
                value: "orange fedora");

            migrationBuilder.InsertData(
                table: "AvatarParts",
                columns: new[] { "Id", "Nombre", "Orden", "RecursoUnity", "Slot" },
                values: new object[,]
                {
                    { 401, "PFP Perro", 0, "pfp_dog", "PFP" },
                    { 402, "PFP Gato", 1, "pfp_cat", "PFP" },
                    { 403, "PFP Robot", 2, "pfp_bot", "PFP" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Perfiles_ProfilePictureId",
                table: "Perfiles",
                column: "ProfilePictureId");

            migrationBuilder.AddForeignKey(
                name: "FK_Perfiles_AvatarParts_ProfilePictureId",
                table: "Perfiles",
                column: "ProfilePictureId",
                principalTable: "AvatarParts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Perfiles_AvatarParts_ProfilePictureId",
                table: "Perfiles");

            migrationBuilder.DropIndex(
                name: "IX_Perfiles_ProfilePictureId",
                table: "Perfiles");

            migrationBuilder.DeleteData(
                table: "AvatarParts",
                keyColumn: "Id",
                keyValue: 401);

            migrationBuilder.DeleteData(
                table: "AvatarParts",
                keyColumn: "Id",
                keyValue: 402);

            migrationBuilder.DeleteData(
                table: "AvatarParts",
                keyColumn: "Id",
                keyValue: 403);

            migrationBuilder.DropColumn(
                name: "ProfilePictureId",
                table: "Perfiles");

            migrationBuilder.UpdateData(
                table: "AvatarParts",
                keyColumn: "Id",
                keyValue: 304,
                column: "RecursoUnity",
                value: "orange hat");
        }
    }
}
