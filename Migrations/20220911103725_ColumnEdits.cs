using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaristaHome.Migrations
{
    public partial class ColumnEdits : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageData",
                table: "User",
                newName: "UserImageData");

            migrationBuilder.RenameColumn(
                name: "ImageData",
                table: "Drink",
                newName: "DrinkImageData");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserImageData",
                table: "User",
                newName: "ImageData");

            migrationBuilder.RenameColumn(
                name: "DrinkImageData",
                table: "Drink",
                newName: "ImageData");
        }
    }
}
