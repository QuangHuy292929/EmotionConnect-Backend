using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infracstructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMatchingQueueState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AssignedAt",
                table: "matching_requests",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AssignedRoomId",
                table: "matching_requests",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "QueuedAt",
                table: "matching_requests",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_matching_requests_AssignedRoomId",
                table: "matching_requests",
                column: "AssignedRoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_matching_requests_rooms_AssignedRoomId",
                table: "matching_requests",
                column: "AssignedRoomId",
                principalTable: "rooms",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_matching_requests_rooms_AssignedRoomId",
                table: "matching_requests");

            migrationBuilder.DropIndex(
                name: "IX_matching_requests_AssignedRoomId",
                table: "matching_requests");

            migrationBuilder.DropColumn(
                name: "AssignedAt",
                table: "matching_requests");

            migrationBuilder.DropColumn(
                name: "AssignedRoomId",
                table: "matching_requests");

            migrationBuilder.DropColumn(
                name: "QueuedAt",
                table: "matching_requests");
        }
    }
}
