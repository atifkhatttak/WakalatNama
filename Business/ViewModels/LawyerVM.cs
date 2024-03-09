using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Business.ViewModels
{
    public class LawyerVM
    {
        public LawyerVM() { }
        public long? Id { get; set; }
        public string? UserName { get; set; }
        public float? TotalExperience { get; set; }
        public float? Rating { get; set; }
        public string? ProfilePic { get; set;}
        public bool? IsFavourite { get; set;}
        public string ProfileDescription { get; set;}
        public int? CompletedCase { get; set; }
        public int? TotalClient { get; set; }
        public long? LawyerId { get; internal set; }
        public long CitizenId { get; internal set; }
    }
    public class CitizenVM
    {
        public long? Id { get; set; }
        public string? UserName { get; set; }
        public string? ProfilePic { get; set; }
        public string? FullName { get; set; }
        public string? FatherName { get; set; }
        public string? Email { get; set; }
        [Required(ErrorMessage ="CNIC is required")]
        public string? CNICNo { get; set; }
        [Required(ErrorMessage = "Phone Number is required")]
        public string? ContactNumber { get; set; }
        [Required(ErrorMessage = "Current address is required")]
        public string? CurrAddress { get; set; }
        [Required(ErrorMessage = "Permanent address is required")]
        public string? PermAddress { get; set; }
        public long UserId { get; set; }
        public long ProfileId { get; set; }
        public IFormFile? CNICFront { get; set; } = null;
        public IFormFile? CNICBack { get; set; } = null;
        public IFormFile? ProfilePhoto { get; set; } = null;
        public string? CountryCode { get; set; }
        public int? CityId { get; set; }
    }
    public class LawyerHomeVM
    {
        public int TotalCases { get; set; }=0;
        public int CompltedCase { get; set; } = 0;
        public List<CourtCaseVM> CourtCases { get; set; }=new List<CourtCaseVM>();

    }
}
