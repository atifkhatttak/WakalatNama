using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DomainModels
{
    public class ChatSession:BaseModel
    {
        [Key]
        public long SessionId { get; set; }
        public long SenderId { get; set; }
        public long ReceiverId { get; set; }
        public long LastMessageId {  get; set; }

    }
}
