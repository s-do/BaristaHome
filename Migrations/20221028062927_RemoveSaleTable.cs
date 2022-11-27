using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaristaHome.Migrations
{
    public partial class RemoveSaleTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sale");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sale",
                columns: table => new
                {
                    SaleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InventoryItemItemId = table.Column<int>(type: "int", nullable: false),
                    InventoryItemStoreId = table.Column<int>(type: "int", nullable: false),
                    Profit = table.Column<decimal>(type: "decimal(16,2)", precision: 16, scale: 2, nullable: false),
                    TimeSold = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UnitsSold = table.Column<decimal>(type: "decimal(16,2)", precision: 16, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sale", x => x.SaleId);
                    table.ForeignKey(
                        name: "FK_Sale_InventoryItem_InventoryItemItemId_InventoryItemStoreId",
                        columns: x => new { x.InventoryItemItemId, x.InventoryItemStoreId },
                        principalTable: "InventoryItem",
                        principalColumns: new[] { "ItemId", "StoreId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sale_InventoryItemItemId_InventoryItemStoreId",
                table: "Sale",
                columns: new[] { "InventoryItemItemId", "InventoryItemStoreId" });
        }
    }
}
