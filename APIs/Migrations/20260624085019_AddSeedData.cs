using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace APIs.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "companies",
                columns: new[] { "CompanyId", "CompanyName", "Industry" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-0000-0000-0000-000000000001"), "Takealot", "Technology" },
                    { new Guid("aaaaaaaa-0000-0000-0000-000000000002"), "Vodacom", "Telecommunications" },
                    { new Guid("aaaaaaaa-0000-0000-0000-000000000003"), "Discovery", "Insurance" },
                    { new Guid("aaaaaaaa-0000-0000-0000-000000000004"), "Standard Bank", "Finance" },
                    { new Guid("aaaaaaaa-0000-0000-0000-000000000005"), "FNB FirstRand", "Finance" },
                    { new Guid("aaaaaaaa-0000-0000-0000-000000000006"), "Media24", "Media" }
                });

            migrationBuilder.InsertData(
                table: "job_listings",
                columns: new[] { "Id", "ClosingDate", "CompanyId", "Description", "Type", "IsActive", "Location", "PostedAt", "SalaryMax", "SalaryMin", "SearchVector", "Title" },
                values: new object[,]
                {
                    { new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), new DateTime(2026, 7, 24, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("aaaaaaaa-0000-0000-0000-000000000001"), "We are looking for a talented Senior Frontend Engineer...", "FullTime", true, "Cape Town", new DateTime(2026, 6, 24, 0, 0, 0, 0, DateTimeKind.Utc), 45000m, 30000m, null, "Senior Frontend Software Engineer" },
                    { new Guid("b2c3d4e5-f6a7-8901-bcde-f12345678901"), new DateTime(2026, 7, 24, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("aaaaaaaa-0000-0000-0000-000000000002"), "We are looking for a Junior Systems Developer...", "FullTime", true, "Johannesburg, Sandton", new DateTime(2026, 6, 21, 0, 0, 0, 0, DateTimeKind.Utc), 30000m, 15000m, null, "Junior Systems Developer" },
                    { new Guid("c3d4e5f6-a7b8-9012-cdef-123456789012"), new DateTime(2026, 7, 24, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("aaaaaaaa-0000-0000-0000-000000000003"), "We are looking for a creative UX/Web Designer...", "Contract", true, "Sandton", new DateTime(2026, 6, 14, 0, 0, 0, 0, DateTimeKind.Utc), 18000m, 10000m, null, "UX/Web Designer" },
                    { new Guid("d4e5f6a7-b8c9-0123-defa-234567890123"), new DateTime(2026, 6, 19, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("aaaaaaaa-0000-0000-0000-000000000004"), "We are looking for a Data Analyst Intern...", "Internship", false, "Pretoria/Hybrid", new DateTime(2026, 5, 10, 0, 0, 0, 0, DateTimeKind.Utc), 22000m, 15000m, null, "Data Analyst Intern" },
                    { new Guid("e5f6a7b8-c9d0-1234-efab-345678901234"), new DateTime(2026, 7, 24, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("aaaaaaaa-0000-0000-0000-000000000005"), "We are looking for a Senior DevOps Engineer...", "FullTime", true, "Bloemfontein", new DateTime(2026, 6, 22, 0, 0, 0, 0, DateTimeKind.Utc), 110000m, 70000m, null, "Senior DevOps Engineer" },
                    { new Guid("f6a7b8c9-d0e1-2345-fabc-456789012345"), new DateTime(2026, 7, 24, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("aaaaaaaa-0000-0000-0000-000000000006"), "We are looking for a Content Writer...", "PartTime", true, "Remote", new DateTime(2026, 4, 25, 0, 0, 0, 0, DateTimeKind.Utc), 18000m, 12000m, null, "Part-Time Content Writer/Promoter" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "job_listings",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"));

            migrationBuilder.DeleteData(
                table: "job_listings",
                keyColumn: "Id",
                keyValue: new Guid("b2c3d4e5-f6a7-8901-bcde-f12345678901"));

            migrationBuilder.DeleteData(
                table: "job_listings",
                keyColumn: "Id",
                keyValue: new Guid("c3d4e5f6-a7b8-9012-cdef-123456789012"));

            migrationBuilder.DeleteData(
                table: "job_listings",
                keyColumn: "Id",
                keyValue: new Guid("d4e5f6a7-b8c9-0123-defa-234567890123"));

            migrationBuilder.DeleteData(
                table: "job_listings",
                keyColumn: "Id",
                keyValue: new Guid("e5f6a7b8-c9d0-1234-efab-345678901234"));

            migrationBuilder.DeleteData(
                table: "job_listings",
                keyColumn: "Id",
                keyValue: new Guid("f6a7b8c9-d0e1-2345-fabc-456789012345"));

            migrationBuilder.DeleteData(
                table: "companies",
                keyColumn: "CompanyId",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "companies",
                keyColumn: "CompanyId",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "companies",
                keyColumn: "CompanyId",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "companies",
                keyColumn: "CompanyId",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "companies",
                keyColumn: "CompanyId",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "companies",
                keyColumn: "CompanyId",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000006"));
        }
    }
}
