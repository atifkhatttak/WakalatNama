using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ViewModels
{
    public class ChatBoxVm
    {
        public long? UserId { get; set; }
        public long? MessageId { get; set; }
        public string? FullName { get; set; }
        public string? Content { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? IsRead { get; set; }
        public string? ImageUrl { get; set; }
        public int UnReadCount { get; set; } 
    }
}
