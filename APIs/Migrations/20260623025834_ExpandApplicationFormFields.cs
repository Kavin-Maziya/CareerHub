using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIs.Migrations
{
    /// <inheritdoc />
    public partial class ExpandApplicationFormFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "job_listings",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<bool>(
                name: "AvailableImmediately",
                table: "applications",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CoverLetter",
                table: "applications",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "applications",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "applications",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LinkedInUrl",
                table: "applications",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NoticePeriodWeeks",
                table: "applications",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "applications",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "YearsOfExperience",
                table: "applications",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableImmediately",
                table: "applications");

            migrationBuilder.DropColumn(
                name: "CoverLetter",
                table: "applications");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "applications");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "applications");

            migrationBuilder.DropColumn(
                name: "LinkedInUrl",
                table: "applications");

            migrationBuilder.DropColumn(
                name: "NoticePeriodWeeks",
                table: "applications");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "applications");

            migrationBuilder.DropColumn(
                name: "YearsOfExperience",
                table: "applications");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "job_listings",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
