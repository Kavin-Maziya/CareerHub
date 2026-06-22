using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace APIs.Migrations
{
    /// <inheritdoc />
    public partial class AddClosingDateAndSearchVector : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.AddColumn<DateTime>(
        name: "ClosingDate",
        table: "job_listings",
        type: "timestamp with time zone",
        nullable: true);

    migrationBuilder.AddColumn<NpgsqlTsVector>(
        name: "SearchVector",
        table: "job_listings",
        type: "tsvector",
        nullable: true);
}

protected override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.DropColumn(name: "ClosingDate", table: "job_listings");
    migrationBuilder.DropColumn(name: "SearchVector", table: "job_listings");
}
    }
}
