using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
    public class CitizenVM
    {
        public long? Id { get; set; }
        public string? UserName { get; set; }
        public string? ProfilePic { get; set; }
        public string? FullName { get; internal set; }
        public string? FatherName { get; internal set; }
        public string? Email { get; internal set; }
        public string? CNICNo { get; internal set; }
        public string? ContactNumber { get; internal set; }
        public string? CurrAddress { get; internal set; }
        public string? PermAddress { get; internal set; }
        public long UserId { get; internal set; }
        public long ProfileId { get; internal set; }
        public IFormFile CNICFront { get; set; } = null;
        public IFormFile CNICBack { get; set; } = null;
        public IFormFile ProfilePhoto { get; set; } = null;
        public string? CountryCode { get; internal set; }
        public int? CityId { get; internal set; }
    }
    public class LawyerHomeVM
    {
        public int TotalCases { get; set; }=0;
        public int CompltedCase { get; set; } = 0;
        public List<CourtCaseVM> CourtCases { get; set; }=new List<CourtCaseVM>();

    }
}
