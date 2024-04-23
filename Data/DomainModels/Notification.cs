using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DomainModels
{
    public class Notification : BaseModel
    {
        public long Id { get; set; }
        public required string Content { get; set; }
        public long FromUserId { get; set; }
        public long ToUserId { get; set; }
        public int NotificationType { get; set; }
        public bool IsRead { get; set; } = false;
    }
}
