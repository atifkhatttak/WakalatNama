using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ViewModels
{
    public class CaseRejectionReasonVm
    {
        public int RejectionId   { get; set; }
        public long CaseId { get; set; }
        public string CaseNumber { get; set; }
        public string CitizenName { get; set; }
        public string CategoryName { get; set; }
        public string Reason { get; set; }
        public int CaseRejectionStatus { get; set; }
    }
}
