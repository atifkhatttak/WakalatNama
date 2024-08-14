using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ViewModels
{
    public class MessageWrapper
    {
        public long UserId { get; set; } 
        public string FullName { get; set; }
        public int UnReadCount { get; set; } = 0;
        public List<MessageVm> Messages { get; set; } = new List<MessageVm>();
    }
}
