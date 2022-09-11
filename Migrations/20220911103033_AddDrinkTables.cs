using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaristaHome.Migrations
{
    public partial class AddDrinkTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Category_Checklists_ChecklistId",
                table: "Category");

            migrationBuilder.DropForeignKey(
                name: "FK_Checklists_Store_StoreId",
                table: "Checklists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Checklists",
                table: "Checklists");

            migrationBuilder.RenameTable(
                name: "Checklists",
                newName: "Checklist");

            migrationBuilder.RenameIndex(
                name: "IX_Checklists_StoreId",
                table: "Checklist",
                newName: "IX_Checklist_StoreId");

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "User",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserImage",
                table: "User",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Checklist",
                table: "Checklist",
                column: "ChecklistId");

            migrationBuilder.CreateTable(
                name: "Drink",
                columns: table => new
                {
                    DrinkId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DrinkName = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Instructions = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    DrinkImage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageData = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drink", x => x.DrinkId);
                });

            migrationBuilder.CreateTable(
                name: "Ingredient",
                columns: table => new
                {
                    IngredientId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IngredientName = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingredient", x => x.IngredientId);
                });

            migrationBuilder.CreateTable(
                name: "Tag",
                columns: table => new
                {
                    TagId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TagName = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => x.TagId);
                });

            migrationBuilder.CreateTable(
                name: "DrinkIngredient",
                columns: table => new
                {
                    DrinkId = table.Column<int>(type: "int", nullable: false),
                    IngredientId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrinkIngredient", x => new { x.DrinkId, x.IngredientId });
                    table.ForeignKey(
                        name: "FK_DrinkIngredient_Drink_DrinkId",
                        column: x => x.DrinkId,
                        principalTable: "Drink",
                        principalColumn: "DrinkId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DrinkIngredient_Ingredient_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "Ingredient",
                        principalColumn: "IngredientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DrinkTag",
                columns: table => new
                {
                    DrinkId = table.Column<int>(type: "int", nullable: false),
                    TagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrinkTag", x => new { x.DrinkId, x.TagId });
                    table.ForeignKey(
                        name: "FK_DrinkTag_Drink_DrinkId",
                        column: x => x.DrinkId,
                        principalTable: "Drink",
                        principalColumn: "DrinkId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DrinkTag_Tag_TagId",
                        column: x => x.TagId,
                        principalTable: "Tag",
                        principalColumn: "TagId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DrinkIngredient_IngredientId",
                table: "DrinkIngredient",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_DrinkTag_TagId",
                table: "DrinkTag",
                column: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_Category_Checklist_ChecklistId",
                table: "Category",
                column: "ChecklistId",
                principalTable: "Checklist",
                principalColumn: "ChecklistId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Checklist_Store_StoreId",
                table: "Checklist",
                column: "StoreId",
                principalTable: "Store",
                principalColumn: "StoreId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Category_Checklist_ChecklistId",
                table: "Category");

            migrationBuilder.DropForeignKey(
                name: "FK_Checklist_Store_StoreId",
                table: "Checklist");

            migrationBuilder.DropTable(
                name: "DrinkIngredient");

            migrationBuilder.DropTable(
                name: "DrinkTag");

            migrationBuilder.DropTable(
                name: "Ingredient");

            migrationBuilder.DropTable(
                name: "Drink");

            migrationBuilder.DropTable(
                name: "Tag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Checklist",
                table: "Checklist");

            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "User");

            migrationBuilder.DropColumn(
                name: "UserImage",
                table: "User");

            migrationBuilder.RenameTable(
                name: "Checklist",
                newName: "Checklists");

            migrationBuilder.RenameIndex(
                name: "IX_Checklist_StoreId",
                table: "Checklists",
                newName: "IX_Checklists_StoreId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Checklists",
                table: "Checklists",
                column: "ChecklistId");

            migrationBuilder.AddForeignKey(
                name: "FK_Category_Checklists_ChecklistId",
                table: "Category",
                column: "ChecklistId",
                principalTable: "Checklists",
                principalColumn: "ChecklistId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Checklists_Store_StoreId",
                table: "Checklists",
                column: "StoreId",
                principalTable: "Store",
                principalColumn: "StoreId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
