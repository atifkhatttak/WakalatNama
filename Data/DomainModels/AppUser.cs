using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DomainModels
{
    public class AppUser : IdentityUser<int>
    {
        public string  FirstName { get; set; }
        public string  LastName { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
        public long UpdatedBy { get; set; }
        public bool IsDeleted { get; set; } = false;
        public int? OTPCode { get; set; } 
    }
}
