using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ViewModels
{
    public class FetchCaseDateNotificationDataVm
    {
        public long CaseId { get; set; }
        public long CaseDateId { get; set; } 
        public long CitizenId { get; set; }
        public long LawyerId { get; set; }
        public string CitizeName { get; set; }
        public string CitizenEmail { get; set; }
        public string CitizenContactNumber { get; set; }
        public string LawerName { get; set; }
        public string LawyerEmail { get; set; }
        public string LawyerContactNumber { get; set; }
        public string CaseTitle { get; set; }
        public string CaseDescription { get; set; }
        public string CaseDateTitle { get; set; }
        public DateTime HearingDate { get; set; }


    }
}
