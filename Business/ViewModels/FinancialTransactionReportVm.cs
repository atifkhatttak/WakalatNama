using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ViewModels
{
    public class FinancialTransactionReportVm
    {
        public string Lawyer { get; set; }
        public string Citizen { get; set; }
        public string CaseTitle { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedDate { get; set; } 

    }
}
