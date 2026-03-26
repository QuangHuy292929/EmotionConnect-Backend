using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infracstructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedCommunities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "communities",
                columns: new[] { "Id", "Category", "CreatedAt", "Description", "IsActive", "Name", "Slug", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("a1f3b2cb-0d53-4e8e-9d6d-6c0be9ec4f11"), "Career", new DateTime(2026, 3, 26, 0, 0, 0, 0, DateTimeKind.Utc), "A supportive space for sharing workplace pressure, career uncertainty, and professional burnout.", true, "Career Support", "career", new DateTime(2026, 3, 26, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("c97d2e9a-1de4-46fe-9300-2a76092e6d22"), "Study", new DateTime(2026, 3, 26, 0, 0, 0, 0, DateTimeKind.Utc), "A community for academic stress, exam anxiety, learning struggles, and student life challenges.", true, "Study Support", "study", new DateTime(2026, 3, 26, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("f45d3de7-9c1f-4f31-bff0-6b67e2f6a733"), "Life", new DateTime(2026, 3, 26, 0, 0, 0, 0, DateTimeKind.Utc), "A place to connect over personal challenges, family matters, loneliness, and everyday emotional burdens.", true, "Life Support", "life", new DateTime(2026, 3, 26, 0, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "communities",
                keyColumn: "Id",
                keyValue: new Guid("a1f3b2cb-0d53-4e8e-9d6d-6c0be9ec4f11"));

            migrationBuilder.DeleteData(
                table: "communities",
                keyColumn: "Id",
                keyValue: new Guid("c97d2e9a-1de4-46fe-9300-2a76092e6d22"));

            migrationBuilder.DeleteData(
                table: "communities",
                keyColumn: "Id",
                keyValue: new Guid("f45d3de7-9c1f-4f31-bff0-6b67e2f6a733"));
        }
    }
}
