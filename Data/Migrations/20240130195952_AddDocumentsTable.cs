using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BarCouncilCardScanBack",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "BarCouncilCardScanFront",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "CNICPicBack",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "CNICPicFront",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "NICOPPic",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "PassportPic",
                table: "UserProfiles");

            migrationBuilder.CreateTable(
                name: "CasesDocuments",
                columns: table => new
                {
                    DocumentId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CaseId = table.Column<long>(type: "bigint", nullable: false),
                    DocName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    DocPath = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    DocExtension = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocTypeId = table.Column<int>(type: "int", nullable: true),
                    IsUploaded = table.Column<bool>(type: "bit", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedUser = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CasesDocuments", x => x.DocumentId);
                });

            migrationBuilder.CreateTable(
                name: "UserDocuments",
                columns: table => new
                {
                    DocumentId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    DocName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    DocPath = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    DocExtension = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocTypeId = table.Column<int>(type: "int", nullable: true),
                    IsUploaded = table.Column<bool>(type: "bit", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedUser = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDocuments", x => x.DocumentId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CasesDocuments");

            migrationBuilder.DropTable(
                name: "UserDocuments");

            migrationBuilder.AddColumn<string>(
                name: "BarCouncilCardScanBack",
                table: "UserProfiles",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BarCouncilCardScanFront",
                table: "UserProfiles",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CNICPicBack",
                table: "UserProfiles",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CNICPicFront",
                table: "UserProfiles",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NICOPPic",
                table: "UserProfiles",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PassportPic",
                table: "UserProfiles",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");
        }
    }
}
