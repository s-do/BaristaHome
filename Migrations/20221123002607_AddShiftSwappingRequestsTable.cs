using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaristaHome.Migrations
{
    /// <inheritdoc />
    public partial class AddShiftSwappingRequestsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "ShiftSwappingRequest",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderUserId = table.Column<int>(type: "int", nullable: false),
                    RecipientUserId = table.Column<int>(type: "int", nullable: false),
                    SenderShiftId = table.Column<int>(type: "int", nullable: false),
                    RecipientShiftId = table.Column<int>(type: "int", nullable: false),
                    Response = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftSwappingRequest", x => x.RequestId);
                    table.ForeignKey(
                        name: "FK_ShiftSwappingRequest_Shift_RecipientShiftId",
                        column: x => x.RecipientShiftId,
                        principalTable: "Shift",
                        principalColumn: "ShiftId",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_ShiftSwappingRequest_Shift_SenderShiftId",
                        column: x => x.SenderShiftId,
                        principalTable: "Shift",
                        principalColumn: "ShiftId",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_ShiftSwappingRequest_User_RecipientUserId",
                        column: x => x.RecipientUserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_ShiftSwappingRequest_User_SenderUserId",
                        column: x => x.SenderUserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShiftSwappingRequest_RecipientShiftId",
                table: "ShiftSwappingRequest",
                column: "RecipientShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftSwappingRequest_RecipientUserId",
                table: "ShiftSwappingRequest",
                column: "RecipientUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftSwappingRequest_SenderShiftId",
                table: "ShiftSwappingRequest",
                column: "SenderShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftSwappingRequest_SenderUserId",
                table: "ShiftSwappingRequest",
                column: "SenderUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShiftSwappingRequest");

           
        }
    }
}
