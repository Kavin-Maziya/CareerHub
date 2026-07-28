using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace APIs.Migrations
{
    /// <inheritdoc />
    public partial class SeedApplicantsAndApplications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "applicants",
                columns: new[] { "Id", "Email", "FirstName", "LastName", "Phone" },
                values: new object[,]
                {
                    { new Guid("bbbbbbbb-0000-0000-0000-000000000001"), "thabo.nkosi@example.com", "Thabo", "Nkosi", "0810000001" },
                    { new Guid("bbbbbbbb-0000-0000-0000-000000000002"), "amanda.vandermerwe@example.com", "Amanda", "van der Merwe", "0810000002" },
                    { new Guid("bbbbbbbb-0000-0000-0000-000000000003"), "sipho.dlamini@example.com", "Sipho", "Dlamini", "0810000003" }
                });

            migrationBuilder.InsertData(
                table: "applications",
                columns: new[] { "ApplicantId", "JobListingId", "AvailableImmediately", "CoverLetter", "Email", "FullName", "LinkedInUrl", "NoticePeriodWeeks", "Phone", "Status", "SubmittedAt", "YearsOfExperience" },
                values: new object[,]
                {
                    { new Guid("bbbbbbbb-0000-0000-0000-000000000001"), new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), true, "I am excited to apply for this role and believe my experience aligns well with what you're looking for.", "thabo.nkosi@example.com", "Thabo Nkosi", null, 0, "0810000000", 0, new DateTime(2026, 7, 10, 0, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { new Guid("bbbbbbbb-0000-0000-0000-000000000002"), new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), false, "I am excited to apply for this role and believe my experience aligns well with what you're looking for.", "amanda.vandermerwe@example.com", "Amanda van der Merwe", null, 4, "0810000000", 1, new DateTime(2026, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), 4 },
                    { new Guid("bbbbbbbb-0000-0000-0000-000000000003"), new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), true, "I am excited to apply for this role and believe my experience aligns well with what you're looking for.", "sipho.dlamini@example.com", "Sipho Dlamini", null, 0, "0810000000", 2, new DateTime(2026, 6, 28, 0, 0, 0, 0, DateTimeKind.Utc), 6 },
                    { new Guid("bbbbbbbb-0000-0000-0000-000000000001"), new Guid("b2c3d4e5-f6a7-8901-bcde-f12345678901"), true, "I am excited to apply for this role and believe my experience aligns well with what you're looking for.", "thabo.nkosi@example.com", "Thabo Nkosi", null, 0, "0810000000", 1, new DateTime(2026, 7, 9, 0, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { new Guid("bbbbbbbb-0000-0000-0000-000000000002"), new Guid("b2c3d4e5-f6a7-8901-bcde-f12345678901"), false, "I am excited to apply for this role and believe my experience aligns well with what you're looking for.", "amanda.vandermerwe@example.com", "Amanda van der Merwe", null, 4, "0810000000", 2, new DateTime(2026, 7, 3, 0, 0, 0, 0, DateTimeKind.Utc), 4 },
                    { new Guid("bbbbbbbb-0000-0000-0000-000000000003"), new Guid("b2c3d4e5-f6a7-8901-bcde-f12345678901"), true, "I am excited to apply for this role and believe my experience aligns well with what you're looking for.", "sipho.dlamini@example.com", "Sipho Dlamini", null, 0, "0810000000", 3, new DateTime(2026, 6, 27, 0, 0, 0, 0, DateTimeKind.Utc), 6 },
                    { new Guid("bbbbbbbb-0000-0000-0000-000000000001"), new Guid("c3d4e5f6-a7b8-9012-cdef-123456789012"), true, "I am excited to apply for this role and believe my experience aligns well with what you're looking for.", "thabo.nkosi@example.com", "Thabo Nkosi", null, 0, "0810000000", 2, new DateTime(2026, 7, 8, 0, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { new Guid("bbbbbbbb-0000-0000-0000-000000000002"), new Guid("c3d4e5f6-a7b8-9012-cdef-123456789012"), false, "I am excited to apply for this role and believe my experience aligns well with what you're looking for.", "amanda.vandermerwe@example.com", "Amanda van der Merwe", null, 4, "0810000000", 3, new DateTime(2026, 7, 2, 0, 0, 0, 0, DateTimeKind.Utc), 4 },
                    { new Guid("bbbbbbbb-0000-0000-0000-000000000003"), new Guid("c3d4e5f6-a7b8-9012-cdef-123456789012"), true, "I am excited to apply for this role and believe my experience aligns well with what you're looking for.", "sipho.dlamini@example.com", "Sipho Dlamini", null, 0, "0810000000", 4, new DateTime(2026, 6, 26, 0, 0, 0, 0, DateTimeKind.Utc), 6 },
                    { new Guid("bbbbbbbb-0000-0000-0000-000000000001"), new Guid("d4e5f6a7-b8c9-0123-defa-234567890123"), true, "I am excited to apply for this role and believe my experience aligns well with what you're looking for.", "thabo.nkosi@example.com", "Thabo Nkosi", null, 0, "0810000000", 3, new DateTime(2026, 7, 7, 0, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { new Guid("bbbbbbbb-0000-0000-0000-000000000002"), new Guid("d4e5f6a7-b8c9-0123-defa-234567890123"), false, "I am excited to apply for this role and believe my experience aligns well with what you're looking for.", "amanda.vandermerwe@example.com", "Amanda van der Merwe", null, 4, "0810000000", 4, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4 },
                    { new Guid("bbbbbbbb-0000-0000-0000-000000000003"), new Guid("d4e5f6a7-b8c9-0123-defa-234567890123"), true, "I am excited to apply for this role and believe my experience aligns well with what you're looking for.", "sipho.dlamini@example.com", "Sipho Dlamini", null, 0, "0810000000", 0, new DateTime(2026, 6, 25, 0, 0, 0, 0, DateTimeKind.Utc), 6 },
                    { new Guid("bbbbbbbb-0000-0000-0000-000000000001"), new Guid("e5f6a7b8-c9d0-1234-efab-345678901234"), true, "I am excited to apply for this role and believe my experience aligns well with what you're looking for.", "thabo.nkosi@example.com", "Thabo Nkosi", null, 0, "0810000000", 4, new DateTime(2026, 7, 6, 0, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { new Guid("bbbbbbbb-0000-0000-0000-000000000002"), new Guid("e5f6a7b8-c9d0-1234-efab-345678901234"), false, "I am excited to apply for this role and believe my experience aligns well with what you're looking for.", "amanda.vandermerwe@example.com", "Amanda van der Merwe", null, 4, "0810000000", 0, new DateTime(2026, 6, 30, 0, 0, 0, 0, DateTimeKind.Utc), 4 },
                    { new Guid("bbbbbbbb-0000-0000-0000-000000000003"), new Guid("e5f6a7b8-c9d0-1234-efab-345678901234"), true, "I am excited to apply for this role and believe my experience aligns well with what you're looking for.", "sipho.dlamini@example.com", "Sipho Dlamini", null, 0, "0810000000", 1, new DateTime(2026, 6, 24, 0, 0, 0, 0, DateTimeKind.Utc), 6 },
                    { new Guid("bbbbbbbb-0000-0000-0000-000000000001"), new Guid("f6a7b8c9-d0e1-2345-fabc-456789012345"), true, "I am excited to apply for this role and believe my experience aligns well with what you're looking for.", "thabo.nkosi@example.com", "Thabo Nkosi", null, 0, "0810000000", 0, new DateTime(2026, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { new Guid("bbbbbbbb-0000-0000-0000-000000000002"), new Guid("f6a7b8c9-d0e1-2345-fabc-456789012345"), false, "I am excited to apply for this role and believe my experience aligns well with what you're looking for.", "amanda.vandermerwe@example.com", "Amanda van der Merwe", null, 4, "0810000000", 1, new DateTime(2026, 6, 29, 0, 0, 0, 0, DateTimeKind.Utc), 4 },
                    { new Guid("bbbbbbbb-0000-0000-0000-000000000003"), new Guid("f6a7b8c9-d0e1-2345-fabc-456789012345"), true, "I am excited to apply for this role and believe my experience aligns well with what you're looking for.", "sipho.dlamini@example.com", "Sipho Dlamini", null, 0, "0810000000", 2, new DateTime(2026, 6, 23, 0, 0, 0, 0, DateTimeKind.Utc), 6 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "applications",
                keyColumns: new[] { "ApplicantId", "JobListingId" },
                keyValues: new object[] { new Guid("bbbbbbbb-0000-0000-0000-000000000001"), new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890") });

            migrationBuilder.DeleteData(
                table: "applications",
                keyColumns: new[] { "ApplicantId", "JobListingId" },
                keyValues: new object[] { new Guid("bbbbbbbb-0000-0000-0000-000000000002"), new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890") });

            migrationBuilder.DeleteData(
                table: "applications",
                keyColumns: new[] { "ApplicantId", "JobListingId" },
                keyValues: new object[] { new Guid("bbbbbbbb-0000-0000-0000-000000000003"), new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890") });

            migrationBuilder.DeleteData(
                table: "applications",
                keyColumns: new[] { "ApplicantId", "JobListingId" },
                keyValues: new object[] { new Guid("bbbbbbbb-0000-0000-0000-000000000001"), new Guid("b2c3d4e5-f6a7-8901-bcde-f12345678901") });

            migrationBuilder.DeleteData(
                table: "applications",
                keyColumns: new[] { "ApplicantId", "JobListingId" },
                keyValues: new object[] { new Guid("bbbbbbbb-0000-0000-0000-000000000002"), new Guid("b2c3d4e5-f6a7-8901-bcde-f12345678901") });

            migrationBuilder.DeleteData(
                table: "applications",
                keyColumns: new[] { "ApplicantId", "JobListingId" },
                keyValues: new object[] { new Guid("bbbbbbbb-0000-0000-0000-000000000003"), new Guid("b2c3d4e5-f6a7-8901-bcde-f12345678901") });

            migrationBuilder.DeleteData(
                table: "applications",
                keyColumns: new[] { "ApplicantId", "JobListingId" },
                keyValues: new object[] { new Guid("bbbbbbbb-0000-0000-0000-000000000001"), new Guid("c3d4e5f6-a7b8-9012-cdef-123456789012") });

            migrationBuilder.DeleteData(
                table: "applications",
                keyColumns: new[] { "ApplicantId", "JobListingId" },
                keyValues: new object[] { new Guid("bbbbbbbb-0000-0000-0000-000000000002"), new Guid("c3d4e5f6-a7b8-9012-cdef-123456789012") });

            migrationBuilder.DeleteData(
                table: "applications",
                keyColumns: new[] { "ApplicantId", "JobListingId" },
                keyValues: new object[] { new Guid("bbbbbbbb-0000-0000-0000-000000000003"), new Guid("c3d4e5f6-a7b8-9012-cdef-123456789012") });

            migrationBuilder.DeleteData(
                table: "applications",
                keyColumns: new[] { "ApplicantId", "JobListingId" },
                keyValues: new object[] { new Guid("bbbbbbbb-0000-0000-0000-000000000001"), new Guid("d4e5f6a7-b8c9-0123-defa-234567890123") });

            migrationBuilder.DeleteData(
                table: "applications",
                keyColumns: new[] { "ApplicantId", "JobListingId" },
                keyValues: new object[] { new Guid("bbbbbbbb-0000-0000-0000-000000000002"), new Guid("d4e5f6a7-b8c9-0123-defa-234567890123") });

            migrationBuilder.DeleteData(
                table: "applications",
                keyColumns: new[] { "ApplicantId", "JobListingId" },
                keyValues: new object[] { new Guid("bbbbbbbb-0000-0000-0000-000000000003"), new Guid("d4e5f6a7-b8c9-0123-defa-234567890123") });

            migrationBuilder.DeleteData(
                table: "applications",
                keyColumns: new[] { "ApplicantId", "JobListingId" },
                keyValues: new object[] { new Guid("bbbbbbbb-0000-0000-0000-000000000001"), new Guid("e5f6a7b8-c9d0-1234-efab-345678901234") });

            migrationBuilder.DeleteData(
                table: "applications",
                keyColumns: new[] { "ApplicantId", "JobListingId" },
                keyValues: new object[] { new Guid("bbbbbbbb-0000-0000-0000-000000000002"), new Guid("e5f6a7b8-c9d0-1234-efab-345678901234") });

            migrationBuilder.DeleteData(
                table: "applications",
                keyColumns: new[] { "ApplicantId", "JobListingId" },
                keyValues: new object[] { new Guid("bbbbbbbb-0000-0000-0000-000000000003"), new Guid("e5f6a7b8-c9d0-1234-efab-345678901234") });

            migrationBuilder.DeleteData(
                table: "applications",
                keyColumns: new[] { "ApplicantId", "JobListingId" },
                keyValues: new object[] { new Guid("bbbbbbbb-0000-0000-0000-000000000001"), new Guid("f6a7b8c9-d0e1-2345-fabc-456789012345") });

            migrationBuilder.DeleteData(
                table: "applications",
                keyColumns: new[] { "ApplicantId", "JobListingId" },
                keyValues: new object[] { new Guid("bbbbbbbb-0000-0000-0000-000000000002"), new Guid("f6a7b8c9-d0e1-2345-fabc-456789012345") });

            migrationBuilder.DeleteData(
                table: "applications",
                keyColumns: new[] { "ApplicantId", "JobListingId" },
                keyValues: new object[] { new Guid("bbbbbbbb-0000-0000-0000-000000000003"), new Guid("f6a7b8c9-d0e1-2345-fabc-456789012345") });

            migrationBuilder.DeleteData(
                table: "applicants",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "applicants",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "applicants",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-0000-0000-0000-000000000003"));
        }
    }
}
