using Data.DomainModels;
using ProjWakalatnama.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
}
