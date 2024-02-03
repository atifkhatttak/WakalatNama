﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ViewModels
{
    public class RegisterViewModel
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }    
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; }
        public string Password { get; set; }
        public string  UserName { get; set; }
        public string RoleName { get; set; }    
        public bool? IsOverseas { get; set; }

        //public string Phone { get; set; }
        public string PhoneNumber { get; set; }
        //public string Address { get; set; }
        //public string City { get; set; }
        //public string State { get; set; }
        //public string PostalCode { get; set; }
        //public string Country { get; set; }


    }
}
