using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DomainModels
{
    public class CaseRejectionReason : BaseModel
    {
        public int Id { get; set; }
        public long CaseId { get; set; }
        public long RejectById { get; set; }
        public string Reason { get; set; }
        public int Status { get; set; }  

    }
}
