using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace APIs.Migrations
{
    /// <inheritdoc />
    public partial class AddCheckConstraintsAndIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ── Check Constraints: job_listings ──────────────────────────────────

            migrationBuilder.AddCheckConstraint(
                name: "ck_job_listings_salary_min_positive",
                table: "job_listings",
                sql: "\"SalaryMin\" IS NULL OR \"SalaryMin\" > 0");

            migrationBuilder.AddCheckConstraint(
                name: "ck_job_listings_salary_max_gt_min",
                table: "job_listings",
                sql: "\"SalaryMin\" IS NULL OR \"SalaryMax\" IS NULL OR \"SalaryMax\" > \"SalaryMin\"");

            migrationBuilder.AddCheckConstraint(
                name: "ck_job_listings_closing_after_posted",
                table: "job_listings",
                sql: "\"ClosingDate\" > \"PostedAt\"");

            // ── Check Constraint: applications ────────────────────────────────────

            migrationBuilder.AddCheckConstraint(
                name: "ck_applications_submitted_at_not_future",
                table: "applications",
                sql: "\"SubmittedAt\" <= now()");

            // ── Part 5: tsvector generated column ─────────────────────────────────
            // EF Core cannot express GENERATED ALWAYS AS for tsvector in Fluent API,
            // so we add it via raw SQL. The column is stored (not virtual) so PostgreSQL
            // maintains it automatically on insert/update without application involvement.
            migrationBuilder.Sql(
                @"ALTER TABLE job_listings
                  ADD COLUMN IF NOT EXISTS ""SearchVector"" tsvector
                  GENERATED ALWAYS AS (
                      to_tsvector('english', coalesce(""Title"", '') || ' ' || coalesce(""Description"", ''))
                  ) STORED;");

            // ── Indexes: job_listings ─────────────────────────────────────────────

            migrationBuilder.CreateIndex(
                name: "ix_job_listings_is_active_closing_date",
                table: "job_listings",
                columns: new[] { "IsActive", "ClosingDate" });

            migrationBuilder.CreateIndex(
                name: "ix_job_listings_company_id_is_active",
                table: "job_listings",
                columns: new[] { "CompanyId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "ix_job_listings_search_vector",
                table: "job_listings",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");

            // ── Indexes: applications ─────────────────────────────────────────────

            migrationBuilder.CreateIndex(
                name: "ix_applications_job_listing_id",
                table: "applications",
                column: "JobListingId");

            migrationBuilder.CreateIndex(
                name: "ix_applications_applicant_id",
                table: "applications",
                column: "ApplicantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop indexes
            migrationBuilder.DropIndex(name: "ix_job_listings_is_active_closing_date", table: "job_listings");
            migrationBuilder.DropIndex(name: "ix_job_listings_company_id_is_active", table: "job_listings");
            migrationBuilder.DropIndex(name: "ix_job_listings_search_vector", table: "job_listings");
            migrationBuilder.DropIndex(name: "ix_applications_job_listing_id", table: "applications");
            migrationBuilder.DropIndex(name: "ix_applications_applicant_id", table: "applications");

            // Drop tsvector column
            migrationBuilder.Sql(@"ALTER TABLE job_listings DROP COLUMN IF EXISTS ""SearchVector"";");

            // Drop check constraints
            migrationBuilder.DropCheckConstraint(name: "ck_job_listings_salary_min_positive", table: "job_listings");
            migrationBuilder.DropCheckConstraint(name: "ck_job_listings_salary_max_gt_min", table: "job_listings");
            migrationBuilder.DropCheckConstraint(name: "ck_job_listings_closing_after_posted", table: "job_listings");
            migrationBuilder.DropCheckConstraint(name: "ck_applications_submitted_at_not_future", table: "applications");
        }
    }
}
