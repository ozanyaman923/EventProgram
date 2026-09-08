using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EventProgram.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDemoEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "Id", "CreatedAtUtc", "DisplayName", "Email", "GoogleSubject", "IsBlocked", "Role" },
                values: new object[] { new Guid("f7ac607a-36ed-4bb6-b952-b8fb76a1f764"), new DateTime(2026, 9, 5, 12, 0, 0, 0, DateTimeKind.Utc), "EventProgram Demo", "demo@eventprogram.local", "eventprogram-demo-owner", false, "User" });

            migrationBuilder.InsertData(
                table: "events",
                columns: new[] { "Id", "AccessPasswordHash", "Capacity", "CreatedAtUtc", "Description", "EndsAtUtc", "OwnerId", "ShareCode", "StartsAtUtc", "Status", "Title", "UpdatedAtUtc", "Visibility" },
                values: new object[,]
                {
                    { new Guid("76a7c9a3-5760-4c41-b272-6f08f2c9f203"), null, 20, new DateTime(2026, 9, 5, 12, 0, 0, 0, DateTimeKind.Utc), "Yalnızca davet bağlantısını alan kişiler için private etkinlik örneği.", new DateTime(2030, 7, 10, 19, 30, 0, 0, DateTimeKind.Utc), new Guid("f7ac607a-36ed-4bb6-b952-b8fb76a1f764"), "c8395aa8141c438d9c740627e8c493c3", new DateTime(2030, 7, 10, 18, 0, 0, 0, DateTimeKind.Utc), "Published", "EventProgram Kapalı Test Oturumu", new DateTime(2026, 9, 5, 12, 0, 0, 0, DateTimeKind.Utc), "Private" },
                    { new Guid("9b412d60-9f32-4c1e-b2b4-938c351d67fe"), null, 80, new DateTime(2026, 9, 5, 12, 0, 0, 0, DateTimeKind.Utc), "Yeni başlayan geliştiriciler için proje fikri, portföy ve kariyer üzerine açık demo etkinliği.", new DateTime(2030, 6, 15, 19, 0, 0, 0, DateTimeKind.Utc), new Guid("f7ac607a-36ed-4bb6-b952-b8fb76a1f764"), "8b52222e9e064b1badc429ed86f8f41a", new DateTime(2030, 6, 15, 17, 0, 0, 0, DateTimeKind.Utc), "Published", "Yazılım Topluluğu Buluşması", new DateTime(2026, 9, 5, 12, 0, 0, 0, DateTimeKind.Utc), "Public" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "events",
                keyColumn: "Id",
                keyValue: new Guid("76a7c9a3-5760-4c41-b272-6f08f2c9f203"));

            migrationBuilder.DeleteData(
                table: "events",
                keyColumn: "Id",
                keyValue: new Guid("9b412d60-9f32-4c1e-b2b4-938c351d67fe"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("f7ac607a-36ed-4bb6-b952-b8fb76a1f764"));
        }
    }
}
