using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddQaulificationAndExpertiesTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BarCouncil",
                table: "UserProfiles");

            migrationBuilder.AddColumn<int>(
                name: "BarCouncilId",
                table: "UserProfiles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BarCouncilNo",
                table: "UserProfiles",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsContestedCopy",
                table: "UserProfiles",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsForeignQualified",
                table: "UserProfiles",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MrTitle",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LawyerExperties",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    CategoryId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LawyerExperties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LawyerQualifications",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DegreeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InstituteName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LawyerQualifications", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LawyerExperties");

            migrationBuilder.DropTable(
                name: "LawyerQualifications");

            migrationBuilder.DropColumn(
                name: "BarCouncilId",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "BarCouncilNo",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "IsContestedCopy",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "IsForeignQualified",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "MrTitle",
                table: "UserProfiles");

            migrationBuilder.AddColumn<string>(
                name: "BarCouncil",
                table: "UserProfiles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
