using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infracstructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCommunityFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_emotion_entries_communities_CommunityId",
                table: "emotion_entries");

            migrationBuilder.DropForeignKey(
                name: "FK_matching_requests_communities_CommunityId",
                table: "matching_requests");

            migrationBuilder.DropForeignKey(
                name: "FK_rooms_communities_CommunityId",
                table: "rooms");

            migrationBuilder.DropTable(
                name: "community_members");

            migrationBuilder.DropTable(
                name: "communities");

            migrationBuilder.DropIndex(
                name: "IX_rooms_CommunityId",
                table: "rooms");

            migrationBuilder.DropIndex(
                name: "IX_matching_requests_CommunityId",
                table: "matching_requests");

            migrationBuilder.DropIndex(
                name: "IX_emotion_entries_CommunityId",
                table: "emotion_entries");

            migrationBuilder.DropColumn(
                name: "CommunityId",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "CommunityId",
                table: "matching_requests");

            migrationBuilder.DropColumn(
                name: "CommunityId",
                table: "emotion_entries");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CommunityId",
                table: "rooms",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CommunityId",
                table: "matching_requests",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CommunityId",
                table: "emotion_entries",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "communities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_communities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "community_members",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CommunityId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Role = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_community_members", x => x.Id);
                    table.ForeignKey(
                        name: "FK_community_members_communities_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "communities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_community_members_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "communities",
                columns: new[] { "Id", "Category", "CreatedAt", "Description", "IsActive", "Name", "Slug", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("a1f3b2cb-0d53-4e8e-9d6d-6c0be9ec4f11"), "Career", new DateTime(2026, 3, 26, 0, 0, 0, 0, DateTimeKind.Utc), "A supportive space for sharing workplace pressure, career uncertainty, and professional burnout.", true, "Career Support", "career", new DateTime(2026, 3, 26, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("c97d2e9a-1de4-46fe-9300-2a76092e6d22"), "Study", new DateTime(2026, 3, 26, 0, 0, 0, 0, DateTimeKind.Utc), "A community for academic stress, exam anxiety, learning struggles, and student life challenges.", true, "Study Support", "study", new DateTime(2026, 3, 26, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("f45d3de7-9c1f-4f31-bff0-6b67e2f6a733"), "Life", new DateTime(2026, 3, 26, 0, 0, 0, 0, DateTimeKind.Utc), "A place to connect over personal challenges, family matters, loneliness, and everyday emotional burdens.", true, "Life Support", "life", new DateTime(2026, 3, 26, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_rooms_CommunityId",
                table: "rooms",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_matching_requests_CommunityId",
                table: "matching_requests",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_emotion_entries_CommunityId",
                table: "emotion_entries",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_communities_Category",
                table: "communities",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_communities_Slug",
                table: "communities",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_community_members_CommunityId_UserId",
                table: "community_members",
                columns: new[] { "CommunityId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_community_members_UserId",
                table: "community_members",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_emotion_entries_communities_CommunityId",
                table: "emotion_entries",
                column: "CommunityId",
                principalTable: "communities",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_matching_requests_communities_CommunityId",
                table: "matching_requests",
                column: "CommunityId",
                principalTable: "communities",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_rooms_communities_CommunityId",
                table: "rooms",
                column: "CommunityId",
                principalTable: "communities",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
