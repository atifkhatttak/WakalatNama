using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class Addsp_GetCitizenLawyers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Lawyer");

            migrationBuilder.Sql(@"--exec sp_GetCitizenLawyers 1004,2,1,10,11
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Asim
-- Create date: 04-03-2024
-- Description:	Get all lawyers either favourite or not for user
-- =============================================
Create PROCEDURE sp_GetCitizenLawyers 
 @UserId bigint,
	@CaseCategoryId int,
	@CityId int,
	@ExperienceMin int,
	@ExperienceMax int
AS
BEGIN
	SET NOCOUNT ON;
Declare @LawyerRole int=4 --Lawyer

SELECT 
u.UserId
,u.FullName
,u.TotalExperience
,u.Rating
,isnull(f.IsFavourite,0) IsFavourite

    FROM UserProfiles u
	left join Favourites f on u.UserId=f.LawyerId and f.UserId=@UserId
    WHERE 
        (u.CityId = @CityId and (u.TotalExperience BETWEEN @ExperienceMin AND @ExperienceMax))
    AND u.RoleId = @LawyerRole
    AND u.IsActive = 1
    AND u.IsVerified = 1
    AND u.IsDeleted = 0;
END
GO
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Laywer");
        }
    }
}
