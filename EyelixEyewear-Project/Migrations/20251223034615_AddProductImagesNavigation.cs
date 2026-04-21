using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EyelixEyewear_Project.Migrations
{
    /// <inheritdoc />
    public partial class AddProductImagesNavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductImageId",
                table: "ProductImages",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 1,
                column: "ProductImageId",
                value: null);

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 2,
                column: "ProductImageId",
                value: null);

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 3,
                column: "ProductImageId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductImageId",
                table: "ProductImages",
                column: "ProductImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_ProductImages_ProductImageId",
                table: "ProductImages",
                column: "ProductImageId",
                principalTable: "ProductImages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_ProductImages_ProductImageId",
                table: "ProductImages");

            migrationBuilder.DropIndex(
                name: "IX_ProductImages_ProductImageId",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "ProductImageId",
                table: "ProductImages");
        }
    }
}
