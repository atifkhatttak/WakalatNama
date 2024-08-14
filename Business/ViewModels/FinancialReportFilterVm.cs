using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ViewModels
{
    public class FinancialReportFilterVm
    {
        public int? LawyerId { get; set; }
        public int? CitizenId { get; set; }
        public string? CaseTitle { get; set; }
        public int? LegalStatusId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }     
    }
}
