using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infracstructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Update_room_for_friendship_feature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserHighId",
                table: "rooms",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserLowId",
                table: "rooms",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_rooms_RoomType_UserLowId_UserHighId",
                table: "rooms",
                columns: new[] { "RoomType", "UserLowId", "UserHighId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_rooms_UserLowId_UserHighId",
                table: "rooms",
                columns: new[] { "UserLowId", "UserHighId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_rooms_RoomType_UserLowId_UserHighId",
                table: "rooms");

            migrationBuilder.DropIndex(
                name: "IX_rooms_UserLowId_UserHighId",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "UserHighId",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "UserLowId",
                table: "rooms");
        }
    }
}
