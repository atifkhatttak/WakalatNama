using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Helpers.ReportQueries
{
    public  class ReportQueries
    {
        
        public static string GetCityZoneCategoryLegalStatusQry()
        {
            string requestQuery = @"SELECT 
                                  
                                  CaseTitle,
                                  Party.StatusName AS Party,
                                  CourtStatus.Status,
                                  CreatedDate,
                                  SystemUser.CreatedBy,
                                  City.CityName AS Location,
                                  CaseJurisdictions.JurisdictionName,
                                  Nature.CategoryName AS Nature
                                 
                                  FROM CourtCases Court

                                  INNER JOIN
                                               (SELECT ID, StatusName FROM dbo.PartyStatuses) Party
                                                   ON Court.PartyId = Party.ID
                                  INNER JOIN
                                               (SELECT StatusId, Status FROM dbo.CaseStatuses) CourtStatus
                                                   ON Court.LegalStatusId = CourtStatus.StatusId
                                  INNER JOIN
                                               (
                                                   SELECT Id,
                                                          CONCAT(FirstName, ' ', LastName) AS CreatedBy
                                                   FROM dbo.AspNetUsers
                                               ) SystemUser
                                                ON Court.CreatedBy = SystemUser.Id
                                  INNER JOIN
                                             (SELECT Id, CityName FROM dbo.Cities) City
                                                 ON Court.CasePlacingId = City.Id
                                  INNER JOIN
                                             (SELECT ID, CategoryName FROM dbo.CaseCategories) Nature
                                                 ON Court.CategoryId = Nature.ID
                                  INNER JOIN
                                              (
                                                  SELECT CaseJurisdictionId,
                                                         JurisdictionName
                                                  FROM dbo.CaseJurisdictions
                                              ) AS CaseJurisdictions
                                                 ON Court.CaseJurisdictionId = CaseJurisdictions.CaseJurisdictionId ";
            return requestQuery;
        }
    }
}
