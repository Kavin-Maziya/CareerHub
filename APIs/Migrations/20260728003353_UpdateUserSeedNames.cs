using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIs.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserSeedNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2026, 7, 28, 0, 33, 28, 895, DateTimeKind.Utc).AddTicks(1542));

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "Email", "Username" },
                values: new object[] { new DateTime(2026, 7, 28, 0, 33, 28, 902, DateTimeKind.Utc).AddTicks(5387), "alice@careerhub.co.za", "alice" });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "Email", "Username" },
                values: new object[] { new DateTime(2026, 7, 28, 0, 33, 28, 902, DateTimeKind.Utc).AddTicks(5514), "bob@careerhub.co.za", "bob" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2026, 7, 22, 22, 6, 42, 352, DateTimeKind.Utc).AddTicks(5203));

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "Email", "Username" },
                values: new object[] { new DateTime(2026, 7, 22, 22, 6, 42, 353, DateTimeKind.Utc).AddTicks(4395), "employer@careerhub.co.za", "employer" });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "Email", "Username" },
                values: new object[] { new DateTime(2026, 7, 22, 22, 6, 42, 353, DateTimeKind.Utc).AddTicks(4455), "applicant@careerhub.co.za", "applicant" });
        }
    }
}
