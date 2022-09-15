using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaristaHome.Migrations
{
    public partial class AddChecklistTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Checklists",
                columns: table => new
                {
                    ChecklistId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChecklistTitle = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Checklists", x => x.ChecklistId);
                    table.ForeignKey(
                        name: "FK_Checklists_Store_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoreTask",
                columns: table => new
                {
                    StoreTaskId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskName = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreTask", x => x.StoreTaskId);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    ChecklistId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.CategoryId);
                    table.ForeignKey(
                        name: "FK_Category_Checklists_ChecklistId",
                        column: x => x.ChecklistId,
                        principalTable: "Checklists",
                        principalColumn: "ChecklistId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CategoryTask",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    StoreTaskId = table.Column<int>(type: "int", nullable: false),
                    IsFinished = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryTask", x => new { x.CategoryId, x.StoreTaskId });
                    table.ForeignKey(
                        name: "FK_CategoryTask_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryTask_StoreTask_StoreTaskId",
                        column: x => x.StoreTaskId,
                        principalTable: "StoreTask",
                        principalColumn: "StoreTaskId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Category_ChecklistId",
                table: "Category",
                column: "ChecklistId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryTask_StoreTaskId",
                table: "CategoryTask",
                column: "StoreTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Checklists_StoreId",
                table: "Checklists",
                column: "StoreId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryTask");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "StoreTask");

            migrationBuilder.DropTable(
                name: "Checklists");
        }
    }
}
