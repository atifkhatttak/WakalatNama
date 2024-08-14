using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRejectionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RejectionReasons",
                columns: table => new
                {
                    RejectionId = table.Column<int>(type: "int", nullable: false),
                    RejectionNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RejectionReasons", x => x.RejectionId);
                });

            migrationBuilder.InsertData(
                table: "RejectionReasons",
                columns: new[] { "RejectionId", "Description", "RejectionNote" },
                values: new object[,]
                {
                    { 1, "Other", "Other" },
                    { 2, "Incomplete documents", "Incomplete documents" },
                    { 3, "Payment Incomplete", "Payment Incomplete" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RejectionReasons");
        }
    }
}
