using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserProfile04022024426 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFavourite",
                table: "UserProfiles",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Rating",
                table: "UserProfiles",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "TotalExperience",
                table: "UserProfiles",
                type: "real",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFavourite",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "TotalExperience",
                table: "UserProfiles");
        }
    }
}
