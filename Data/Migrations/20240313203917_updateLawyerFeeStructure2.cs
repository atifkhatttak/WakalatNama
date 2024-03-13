using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class updateLawyerFeeStructure2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "test",
                table: "LawyerFeeStructures");

            migrationBuilder.RenameColumn(
                name: "LawyerFees",
                table: "LawyerFeeStructures",
                newName: "LawyerFee");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LawyerFee",
                table: "LawyerFeeStructures",
                newName: "LawyerFees");

            migrationBuilder.AddColumn<bool>(
                name: "test",
                table: "LawyerFeeStructures",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
