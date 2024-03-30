using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCaseDetailIdinCaseDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CaseDetailId",
                table: "CasesDetails");

            migrationBuilder.AddColumn<long>(
                name: "CaseDetailId",
                table: "CasesDocuments",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CaseDetailId",
                table: "CasesDocuments");

            migrationBuilder.AddColumn<long>(
                name: "CaseDetailId",
                table: "CasesDetails",
                type: "bigint",
                nullable: true);
        }
    }
}
