using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaristaHome.Migrations
{
    public partial class DeleteSale2 : Migration
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
                    DrinkId = table.Column<int>(type: "int", nullable: true),
                    StoreId = table.Column<int>(type: "int", nullable: true),
                    Profit = table.Column<decimal>(type: "decimal(16,2)", precision: 16, scale: 2, nullable: true),
                    TimeSold = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UnitsSold = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sale", x => x.SaleId);
                    table.ForeignKey(
                        name: "FK_Sale_Drink_DrinkId",
                        column: x => x.DrinkId,
                        principalTable: "Drink",
                        principalColumn: "DrinkId");
                    table.ForeignKey(
                        name: "FK_Sale_Store_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "StoreId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sale_DrinkId",
                table: "Sale",
                column: "DrinkId");

            migrationBuilder.CreateIndex(
                name: "IX_Sale_StoreId",
                table: "Sale",
                column: "StoreId");
        }
    }
}
