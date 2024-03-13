using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class updateLawyerFeeStructure1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LawyerFee",
                table: "LawyerFeeStructures");

            migrationBuilder.AddColumn<decimal>(
                name: "LawyerFees",
                table: "LawyerFeeStructures",
                type: "decimal(16,3)",
                precision: 16,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "test",
                table: "LawyerFeeStructures",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LawyerFees",
                table: "LawyerFeeStructures");

            migrationBuilder.DropColumn(
                name: "test",
                table: "LawyerFeeStructures");

            migrationBuilder.AddColumn<decimal>(
                name: "LawyerFee",
                table: "LawyerFeeStructures",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
