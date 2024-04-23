using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedStatusColumnCourtCases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CaseStatusId",
                table: "CourtCases",
                newName: "StatusId");

            migrationBuilder.AddColumn<int>(
                name: "LegalStatusId",
                table: "CourtCases",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LegalStatusId",
                table: "CourtCases");

            migrationBuilder.RenameColumn(
                name: "StatusId",
                table: "CourtCases",
                newName: "CaseStatusId");
        }
    }
}
