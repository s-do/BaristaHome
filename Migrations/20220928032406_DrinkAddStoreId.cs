using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaristaHome.Migrations
{
    public partial class DrinkAddStoreId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "Drink",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Drink_StoreId",
                table: "Drink",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Drink_Store_StoreId",
                table: "Drink",
                column: "StoreId",
                principalTable: "Store",
                principalColumn: "StoreId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drink_Store_StoreId",
                table: "Drink");

            migrationBuilder.DropIndex(
                name: "IX_Drink_StoreId",
                table: "Drink");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "Drink");
        }
    }
}
