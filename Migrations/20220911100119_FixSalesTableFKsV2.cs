using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaristaHome.Migrations
{
    public partial class FixSalesTableFKsV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "Sale");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "Sale");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ItemId",
                table: "Sale",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "Sale",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
