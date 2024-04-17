using Data.DomainModels;
using ProjWakalatnama.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ViewModels
{
  
    public class CasesDropDownVM
    {
        public List<CategoryVM> CaseNature { get; set; } = new List<CategoryVM>();
        public List<PartyStatusVM> PartyStatuses { get; set; } = new List<PartyStatusVM>();
        public List<CaseJurisdictionVM> CaseJurisdictions { get; set; } = new List<CaseJurisdictionVM>();
        public List<CityVM> Cities{ get; set; }=new List<CityVM>();
        public List<CasePursuedVM> CasePursueds { get; set; } = new List<CasePursuedVM>();
    }

    public class CaseJurisdictionVM
    {
        public int ID { get; set; }
        public string CaseJurisdiction { get; set; }
    }
    public class CasePursuedVM
    {
        public int ID { get; set; }
        public string CasePursued { get; set; }
    }
    public class CityVM
    {
        public int ID { get; set; }
        public string CityName { get; set; }
    }
    public class PartyStatusVM
    {
        public int ID { get; set; }
        public string StatusName { get; set; }
    }
    public class CaseStatusesVM
    {
        public int ID { get; set; }
        public string Status { get; set; }
    }

    //Documents
    public class DownloadableDocsVM
    {
        public long? DocId { get; set; }
        public string? DocName { get; set; }
        public double? DocSize { get; set; }
        public int DocType { get; set; }
        public int? DocForUserType { get; set; }
        public string? DocPath { get; internal set; }
    }

    public class UserDocumentVM
    {
        public long DocumentId { get; set; }
        [Required]
        public long UserId { get; set; }
        [StringLength(250)]
        public string? DocName { get; set; }
        [StringLength(250)]
        public string? DocPath { get; set; }
        public string? DocExtension { get; set; }
        public int? DocTypeId { get; set; }
        public bool? IsUploaded { get; set; }
    }
}
