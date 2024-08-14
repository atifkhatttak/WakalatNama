using ProjWakalatnama.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Business.ViewModels
{
    public class MessageVm
    {
        public long Id { get; set; }
        public string Content { get; set; }
        public long FromUserId { get; set; }
        public long ToUserId { get; set; }
        public bool IsRead { get; set; }
        public long? ParentId { get; set; }
        public DateTime? DateTime { get; set; }
    }
    public class ChatHistoryVM:BaseModelVM
    {
        public long Id { get; set; }
        public string? Content { get; set; }
        public string? FromUserName { get; set; }
        public string? ToUserName { get; set; }
        public long? FromUserId { get; set; }
        public long? ToUserId { get; set; }
        public bool? IsRead { get; set; }
        public long? ParentId { get; set; }
    }
}
