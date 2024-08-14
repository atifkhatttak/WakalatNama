using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ViewModels
{
    public class RegisterViewModel
    {
        public long UserId { get; set; }
        [Required(ErrorMessage = "First Name is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "FirstName must be between 1 and 100 characters")]
        public string FirstName { get; set; }        
        public string LastName { get; set; } = string.Empty;
        [Required(ErrorMessage ="Email is required")]
        [MaxLength(100, ErrorMessage = "Maximum length is 100")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 20 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()-_=+{};:,<.>]).{6,20}$",
        ErrorMessage = "Password must contain at least one lowercase letter, one uppercase letter, one digit, and one special character.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "UserName is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "UserName must be between 1 and 100 characters")]
        public string  UserName { get; set; }
        [Required(ErrorMessage = "Role is required")]
        public string RoleName { get; set; }
        //public IFormFile CNICFront { get; set; } = null;
        //public IFormFile CNICBack { get; set; } = null;
        //public IFormFile ProfilePhoto { get; set; } = null;
        //public IFormFileCollection Documents { get; set; } = null;
        [DefaultValue(false)]
        public bool? IsOverseas { get; set; }

        //public string Phone { get; set; }
        [Required(ErrorMessage = "Phone number is required")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$", ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; }
        //public string Address { get; set; }
        public int CityId { get; set; } = -1;
        public long? UserProfileId { get; set; }
        //public string State { get; set; }
        //public string PostalCode { get; set; }
        //public string Country { get; set; }


    }
}
