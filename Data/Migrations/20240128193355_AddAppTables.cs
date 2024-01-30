using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAppTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CaseCategories",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseCategories", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CaseJurisdictions",
                columns: table => new
                {
                    CaseJurisdictionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JurisdictionName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseJurisdictions", x => x.CaseJurisdictionId);
                });

            migrationBuilder.CreateTable(
                name: "CasesDetails",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CaseId = table.Column<long>(type: "bigint", nullable: false),
                    CaseStatusId = table.Column<int>(type: "int", nullable: false),
                    DateDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedUser = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedUser = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CasesDetails", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ConsultationOptions",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConsultationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsultationOptions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CourtCases",
                columns: table => new
                {
                    CaseId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CitizenId = table.Column<long>(type: "bigint", nullable: false),
                    LawyerId = table.Column<long>(type: "bigint", nullable: false),
                    RedundantLawyerId = table.Column<long>(type: "bigint", nullable: false),
                    CaseNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PartyId = table.Column<int>(type: "int", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    CaseDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CaseJurisdictionId = table.Column<int>(type: "int", nullable: true),
                    CourtId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedUser = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedUser = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourtCases", x => x.CaseId);
                });

            migrationBuilder.CreateTable(
                name: "ExperienceCosts",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExperienceMin = table.Column<int>(type: "int", nullable: true),
                    ExperienceMax = table.Column<int>(type: "int", nullable: true),
                    CostMin = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CostMax = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExperienceCosts", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PartyStatuses",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartyStatuses", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    ProfileId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CNICNo = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    CNICPicFront = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CNICPicBack = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ContactNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CurrAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PermAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: true),
                    CountryId = table.Column<int>(type: "int", nullable: true),
                    ProfilePic = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsOverseas = table.Column<bool>(type: "bit", nullable: true),
                    NICOP = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NICOPPic = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    PassportID = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    PassportPic = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ResideCountryId = table.Column<int>(type: "int", nullable: true),
                    OverseasContactNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    OfficeAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LCourtName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LCourtLocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LHighCourtName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LHighCourtLocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Qualification = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Institute = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    BarCouncil = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EnrollmentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BarCouncilCardScanFront = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    BarCouncilCardScanBack = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    AreasOfExpertise = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    AreasOfExpertiseOrther = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAlert = table.Column<bool>(type: "bit", nullable: true),
                    IsSMS = table.Column<bool>(type: "bit", nullable: true),
                    IsEmail = table.Column<bool>(type: "bit", nullable: true),
                    IsPushAlert = table.Column<bool>(type: "bit", nullable: true),
                    IsCreateMeeting = table.Column<bool>(type: "bit", nullable: true),
                    IsAgreed = table.Column<bool>(type: "bit", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUser = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedUser = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    IsVerified = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.ProfileId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CaseCategories");

            migrationBuilder.DropTable(
                name: "CaseJurisdictions");

            migrationBuilder.DropTable(
                name: "CasesDetails");

            migrationBuilder.DropTable(
                name: "ConsultationOptions");

            migrationBuilder.DropTable(
                name: "CourtCases");

            migrationBuilder.DropTable(
                name: "ExperienceCosts");

            migrationBuilder.DropTable(
                name: "PartyStatuses");

            migrationBuilder.DropTable(
                name: "UserProfiles");
        }
    }
}
