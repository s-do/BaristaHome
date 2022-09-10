using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaristaHome.Migrations
{
    public partial class AddUserShiftStatusTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShiftStatusUser");

            migrationBuilder.CreateTable(
                name: "UserShiftStatus",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ShiftStatusId = table.Column<int>(type: "int", nullable: false),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserShiftStatus", x => new { x.UserId, x.ShiftStatusId, x.Time });
                    table.ForeignKey(
                        name: "FK_UserShiftStatus_ShiftStatus_ShiftStatusId",
                        column: x => x.ShiftStatusId,
                        principalTable: "ShiftStatus",
                        principalColumn: "ShiftStatusId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserShiftStatus_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserShiftStatus_ShiftStatusId",
                table: "UserShiftStatus",
                column: "ShiftStatusId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserShiftStatus");

            migrationBuilder.CreateTable(
                name: "ShiftStatusUser",
                columns: table => new
                {
                    ShiftStatusesShiftStatusId = table.Column<int>(type: "int", nullable: false),
                    UsersUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftStatusUser", x => new { x.ShiftStatusesShiftStatusId, x.UsersUserId });
                    table.ForeignKey(
                        name: "FK_ShiftStatusUser_ShiftStatus_ShiftStatusesShiftStatusId",
                        column: x => x.ShiftStatusesShiftStatusId,
                        principalTable: "ShiftStatus",
                        principalColumn: "ShiftStatusId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShiftStatusUser_User_UsersUserId",
                        column: x => x.UsersUserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShiftStatusUser_UsersUserId",
                table: "ShiftStatusUser",
                column: "UsersUserId");
        }
    }
}
