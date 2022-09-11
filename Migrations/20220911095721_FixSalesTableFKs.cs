using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaristaHome.Migrations
{
    public partial class FixSalesTableFKs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItem_Sale_SaleId",
                table: "InventoryItem");

            migrationBuilder.DropIndex(
                name: "IX_InventoryItem_SaleId",
                table: "InventoryItem");

            migrationBuilder.DropColumn(
                name: "SaleId",
                table: "InventoryItem");

            migrationBuilder.AlterColumn<int>(
                name: "StoreId",
                table: "Sale",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<int>(
                name: "ItemId",
                table: "Sale",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("Relational:ColumnOrder", 0);

            migrationBuilder.AddColumn<int>(
                name: "InventoryItemItemId",
                table: "Sale",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "InventoryItemStoreId",
                table: "Sale",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Sale_InventoryItemItemId_InventoryItemStoreId",
                table: "Sale",
                columns: new[] { "InventoryItemItemId", "InventoryItemStoreId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Sale_InventoryItem_InventoryItemItemId_InventoryItemStoreId",
                table: "Sale",
                columns: new[] { "InventoryItemItemId", "InventoryItemStoreId" },
                principalTable: "InventoryItem",
                principalColumns: new[] { "ItemId", "StoreId" },
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sale_InventoryItem_InventoryItemItemId_InventoryItemStoreId",
                table: "Sale");

            migrationBuilder.DropIndex(
                name: "IX_Sale_InventoryItemItemId_InventoryItemStoreId",
                table: "Sale");

            migrationBuilder.DropColumn(
                name: "InventoryItemItemId",
                table: "Sale");

            migrationBuilder.DropColumn(
                name: "InventoryItemStoreId",
                table: "Sale");

            migrationBuilder.AlterColumn<int>(
                name: "StoreId",
                table: "Sale",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<int>(
                name: "ItemId",
                table: "Sale",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 0);

            migrationBuilder.AddColumn<int>(
                name: "SaleId",
                table: "InventoryItem",
                type: "int",
                nullable: true);

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
    }
}
