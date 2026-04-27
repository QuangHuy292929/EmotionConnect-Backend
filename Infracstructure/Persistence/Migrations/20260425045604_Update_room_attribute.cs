using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infracstructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Update_room_attribute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_matching_requests_rooms_AssignedRoomId",
                table: "matching_requests");

            migrationBuilder.AddColumn<int>(
                name: "MinMembers",
                table: "rooms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReadyAt",
                table: "rooms",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_matching_requests_rooms_AssignedRoomId",
                table: "matching_requests",
                column: "AssignedRoomId",
                principalTable: "rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_matching_requests_rooms_AssignedRoomId",
                table: "matching_requests");

            migrationBuilder.DropColumn(
                name: "MinMembers",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "ReadyAt",
                table: "rooms");

            migrationBuilder.AddForeignKey(
                name: "FK_matching_requests_rooms_AssignedRoomId",
                table: "matching_requests",
                column: "AssignedRoomId",
                principalTable: "rooms",
                principalColumn: "Id");
        }
    }
}
