using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaristaHome.Migrations
{
    public partial class AddSaleTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SaleId",
                table: "InventoryItem",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Sale",
                columns: table => new
                {
                    SaleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnitsSold = table.Column<decimal>(type: "decimal(16,2)", precision: 16, scale: 2, nullable: false),
                    Profit = table.Column<decimal>(type: "decimal(16,2)", precision: 16, scale: 2, nullable: false),
                    TimeSold = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sale", x => x.SaleId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItem_SaleId",
                table: "InventoryItem",
                column: "SaleId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItem_Sale_SaleId",
                table: "InventoryItem",
                column: "SaleId",
                principalTable: "Sale",
                principalColumn: "SaleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItem_Sale_SaleId",
                table: "InventoryItem");

            migrationBuilder.DropTable(
                name: "Sale");

            migrationBuilder.DropIndex(
                name: "IX_InventoryItem_SaleId",
                table: "InventoryItem");

            migrationBuilder.DropColumn(
                name: "SaleId",
                table: "InventoryItem");
        }
    }
}
