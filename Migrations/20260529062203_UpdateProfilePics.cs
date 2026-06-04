using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ResiduosBackend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProfilePics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AvatarParts",
                keyColumn: "Id",
                keyValue: 401,
                columns: new[] { "Nombre", "RecursoUnity" },
                values: new object[] { "PFP Oso", "pfp_bear" });

            migrationBuilder.UpdateData(
                table: "AvatarParts",
                keyColumn: "Id",
                keyValue: 402,
                columns: new[] { "Nombre", "RecursoUnity" },
                values: new object[] { "PFP Pollo", "pfp_chicken" });

            migrationBuilder.UpdateData(
                table: "AvatarParts",
                keyColumn: "Id",
                keyValue: 403,
                columns: new[] { "Nombre", "RecursoUnity" },
                values: new object[] { "PFP Koala", "pfp_koala" });

            migrationBuilder.InsertData(
                table: "AvatarParts",
                columns: new[] { "Id", "Nombre", "Orden", "RecursoUnity", "Slot" },
                values: new object[,]
                {
                    { 404, "PFP Suricata", 3, "pfp_meerkat", "PFP" },
                    { 405, "PFP Panda", 4, "pfp_panda", "PFP" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AvatarParts",
                keyColumn: "Id",
                keyValue: 404);

            migrationBuilder.DeleteData(
                table: "AvatarParts",
                keyColumn: "Id",
                keyValue: 405);

            migrationBuilder.UpdateData(
                table: "AvatarParts",
                keyColumn: "Id",
                keyValue: 401,
                columns: new[] { "Nombre", "RecursoUnity" },
                values: new object[] { "PFP Perro", "pfp_dog" });

            migrationBuilder.UpdateData(
                table: "AvatarParts",
                keyColumn: "Id",
                keyValue: 402,
                columns: new[] { "Nombre", "RecursoUnity" },
                values: new object[] { "PFP Gato", "pfp_cat" });

            migrationBuilder.UpdateData(
                table: "AvatarParts",
                keyColumn: "Id",
                keyValue: 403,
                columns: new[] { "Nombre", "RecursoUnity" },
                values: new object[] { "PFP Robot", "pfp_bot" });
        }
    }
}
