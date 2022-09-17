using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaristaHome.Migrations
{
    public partial class AddUserColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserDescription",
                table: "User",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Wage",
                table: "User",
                type: "decimal(16,2)",
                precision: 16,
                scale: 2,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserDescription",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Wage",
                table: "User");
        }
    }
}
