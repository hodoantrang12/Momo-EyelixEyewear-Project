using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EyelixEyewear_Project.Migrations
{
    /// <inheritdoc />
    public partial class AddCollectionsAndRemoveGender : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "CollectionId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Collections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BannerImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collections", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Collections",
                columns: new[] { "Id", "BannerImageUrl", "CreatedAt", "Description", "DisplayOrder", "IsActive", "Name", "Slug" },
                values: new object[,]
                {
                    { 1, "/images/collections/fall-winter-banner.jpg", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Our newest collection featuring warm tones and cozy designs for the fall and winter season.", 1, true, "Fall/Winter 2025", "fall-winter-2025" },
                    { 2, "/images/collections/aluminium-banner.jpg", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Eco-friendly eyewear crafted from 100% recycled aluminium materials.", 2, true, "Recycled Aluminium", "recycled-aluminium" }
                });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "CollectionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CollectionId", "Color", "FrameMaterial" },
                values: new object[] { null, "Wood", "Bio Acetate" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "CollectionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CollectionId", "Color", "FrameShape" },
                values: new object[] { null, "Black", "Squared" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CollectionId", "Color", "Description", "FrameShape" },
                values: new object[] { null, "Black", "Stand out from the crowd with Kala polarized sunglasses. Our sleek and stylish unisex frames are made from 95% recycled metal. ", "Rounded" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CollectionId", "FrameMaterial", "Stock" },
                values: new object[] { null, "Bio Acetate", 150 });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "CollectionId", "Color", "CreatedAt", "Description", "DiscountPrice", "FrameMaterial", "FrameShape", "ImageUrl", "IsActive", "LensMaterial", "Name", "Price", "SKU", "Stock", "UpdatedAt" },
                values: new object[,]
                {
                    { 8, 1, 2, "Olive", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "With its modern geometric silhouette and slim recycled metal frame, Mistral strikes the perfect balance between retro charm and contemporary style. Lightweight yet durable, these glasses are designed to complement any look with ease.", null, "Recycled Metal", "Rounded", "/images/products/mistral.png", true, null, "Mistral", 450000m, "GL-003", 300, null },
                    { 9, 1, 1, "Rasberry", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Aura plays with geometric angles in a softened hexagonal shape – unexpected and chic. Complete your style with the seamless magnetic clip-on, perfect for sunny days.", null, "Recycled Metal", "Squared", "/images/products/aura.png", true, null, "Aura", 550000m, "GL-004", 300, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CollectionId",
                table: "Products",
                column: "CollectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Collections_CollectionId",
                table: "Products",
                column: "CollectionId",
                principalTable: "Collections",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Collections_CollectionId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "Collections");

            migrationBuilder.DropIndex(
                name: "IX_Products_CollectionId",
                table: "Products");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DropColumn(
                name: "CollectionId",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "Gender",
                value: null);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Color", "FrameMaterial", "Gender" },
                values: new object[] { "Tortoise", "Recycled Metal", null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "Gender",
                value: null);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Color", "FrameShape", "Gender" },
                values: new object[] { "Brown", "Square", null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Color", "Description", "FrameShape", "Gender" },
                values: new object[] { "Gold", "Stand out from the crowd with Kala polarized sunglasses. Premium feel.", "Aviator", null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "FrameMaterial", "Gender", "Stock" },
                values: new object[] { "Plastic", null, 10 });
        }
    }
}
