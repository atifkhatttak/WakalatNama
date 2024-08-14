using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DomainModels
{
    public class EmailLogs :BaseModel
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Email { get; set; }
        public string Content { get; set; }
        public string Subject { get; set; }
        public long? CaseId { get; set; } 
        public long? CaseDateId { get; set; }
        public bool IsSent { get; set; }
        public string? ErrorMessage { get; set; } 
        public int SendMode { get; set; } //Scheduled or Instant
        public int NotificationType { get; set; }
    }
}
