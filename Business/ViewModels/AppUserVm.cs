using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Business.ViewModels
{
    public class AppUserVm
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int OTPCode { get; set; }

    }
    public class UserClaimVM
    {
        public long UserId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public List<string> Roles { get; set; }
    }
    public class UserRolesVM
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }

    }
}
