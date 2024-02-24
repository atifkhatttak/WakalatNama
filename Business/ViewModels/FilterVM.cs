using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ViewModels
{
    public class FilterVM
    {
        public int? CaseCategoryId { get; set; }
        public int? CityId { get; set; }
        public int? ExperienceMin { get; set;}
        public int? ExperienceMax { get; set; }
    }
    public class AcceptRejectCaseVM
    {
        public int Status { get; set; }
        public string Reason { get; set; }
        public long CaseId { get; set; }
    }
}
