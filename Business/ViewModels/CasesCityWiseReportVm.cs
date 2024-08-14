using Business.Helpers.Attributes;
using ProjWakalatnama.DataLayer.Models;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Business.ViewModels
{
    public class CasesCityWiseReportVm : SQLBasedVm
    {
        [SQLColumn(Name = "CaseTitle",IsContain =true)] 
        public string? CaseTitle { get; set; }
        [SQLColumn(Name = "Party.StatusName")]
        public string? Party { get; set; }

        #region Output Param Only

        [SQLColumn(Name = "CourtStatus.Status"),SwaggerSchema(ReadOnly =true)]
        public string? Status { get; set; }
        [SQLColumn(Name = "Court.CreatedDate"),DefaultValue(null)]
        public DateTime? CreatedDate { get; set; } = null;
        [SQLColumn(Name = "SystemUser.CreatedBy")]
        public string? CreatedBy { get; set; }
        [SQLColumn(Name = "City.CityName"), SwaggerSchema(ReadOnly = true)]
        public string? Location { get; set; }
        [SQLColumn(Name = "CaseJurisdictions.JurisdictionName"),SwaggerSchema(ReadOnly =true)]
        public string? JurisdictionName { get; set; }
        [SQLColumn(Name = "Nature.CategoryName"),SwaggerSchema(ReadOnly =true)]
        public string? Nature { get; set; }

        #endregion

        #region InPut Param Only

        [SQLColumn(Name = "CaseJurisdictions.CaseJurisdictionId"),DefaultValue(null),SwaggerSchema(WriteOnly =true),NotMapped]
        public int? CaseJurisdictionId { get; set; }

        [SQLColumn(Name = "City.Id"),DefaultValue(null), SwaggerSchema(WriteOnly = true), NotMapped]
        public int? CityId { get; set; }

        [SQLColumn(Name = "Nature.ID"),DefaultValue(null), SwaggerSchema(WriteOnly = true), NotMapped]
        public int? CategoryId { get; set; }

        [SQLColumn(Name = "Court.LegalStatusId"), DefaultValue(null), SwaggerSchema(WriteOnly = true), NotMapped]
        public int? CaseLegalStatusId { get; set; }

        #endregion

        [JsonIgnore]
        public   override string? ReportType { get; } = "GetCityZoneCategoryLegalStatusQry";




    }
}
