using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infracstructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCheckInSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_text_embeddings_Embedding",
                table: "text_embeddings");

            migrationBuilder.AddColumn<string>(
                name: "GoogleId",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsGoogleAccount",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "checkin_sessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmotionEntryId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CurrentStep = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    InputMode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    EmotionAnswer = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IssueAnswer = table.Column<string>(type: "character varying(3000)", maxLength: 3000, nullable: true),
                    DeepDiveAnswer = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    GeneratedSummary = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    EditedSummary = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    ConfirmedSummary = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_checkin_sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_checkin_sessions_emotion_entries_EmotionEntryId",
                        column: x => x.EmotionEntryId,
                        principalTable: "emotion_entries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_checkin_sessions_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_embeddings_hnsw_cosine",
                table: "text_embeddings",
                column: "Embedding")
                .Annotation("Npgsql:IndexMethod", "hnsw")
                .Annotation("Npgsql:IndexOperators", new[] { "vector_cosine_ops" })
                .Annotation("Npgsql:StorageParameter:ef_construction", 100)
                .Annotation("Npgsql:StorageParameter:m", 24);

            migrationBuilder.CreateIndex(
                name: "IX_checkin_sessions_EmotionEntryId",
                table: "checkin_sessions",
                column: "EmotionEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_checkin_sessions_UserId",
                table: "checkin_sessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_checkin_sessions_UserId_CreatedAt",
                table: "checkin_sessions",
                columns: new[] { "UserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_checkin_sessions_UserId_Status",
                table: "checkin_sessions",
                columns: new[] { "UserId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "checkin_sessions");

            migrationBuilder.DropIndex(
                name: "ix_embeddings_hnsw_cosine",
                table: "text_embeddings");

            migrationBuilder.DropColumn(
                name: "GoogleId",
                table: "users");

            migrationBuilder.DropColumn(
                name: "IsGoogleAccount",
                table: "users");

            migrationBuilder.CreateIndex(
                name: "IX_text_embeddings_Embedding",
                table: "text_embeddings",
                column: "Embedding")
                .Annotation("Npgsql:IndexMethod", "ivfflat")
                .Annotation("Npgsql:IndexOperators", new[] { "vector_cosine_ops" });
        }
    }
}
