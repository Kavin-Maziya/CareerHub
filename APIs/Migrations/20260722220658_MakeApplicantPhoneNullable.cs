using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIs.Migrations
{
    /// <inheritdoc />
    public partial class MakeApplicantPhoneNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                column: "CreatedAt",
                value: new DateTime(2026, 7, 22, 22, 6, 42, 353, DateTimeKind.Utc).AddTicks(4395));

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAt",
                value: new DateTime(2026, 7, 22, 22, 6, 42, 353, DateTimeKind.Utc).AddTicks(4455));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2026, 7, 22, 21, 47, 11, 223, DateTimeKind.Utc).AddTicks(9492));

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAt",
                value: new DateTime(2026, 7, 22, 21, 47, 11, 224, DateTimeKind.Utc).AddTicks(6916));

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAt",
                value: new DateTime(2026, 7, 22, 21, 47, 11, 224, DateTimeKind.Utc).AddTicks(6954));
        }
    }
}
