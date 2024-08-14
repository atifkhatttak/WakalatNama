using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ViewModels
{
    public class NotificationVm
    {
        public long Id { get; set; }
        public string Content { get; set; }
        public long FromUserId { get; set; }
        public long ToUserId { get; set; }
        public int NotificationType { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedDate { set; get; }
        public string? ImageUrl { get; set; }
    }
    public class DeviceTokenVM
    {
        [Required]
        public string Token { get; set; }
    }
}
