using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
    public class CitizenHomeVM
    {
        public List<LawyerVM> Lawyers { get; set; }=new List<LawyerVM>();
        public List<LawyerVM> PopularLawyers { get; set; }=new List<LawyerVM>();

    }
    public class LawyerHomeVM
    {
        public int TotalCases { get; set; } = 0;
        public int CompltedCase { get; set; } = 0;
        public List<CourtCaseVM> CourtCases { get; set; } = new List<CourtCaseVM>();

    }

    public class LawyerProfileVM
    {
        public long ProfileId { get; internal set; }
        public string? FullName { get; internal set; }
        public string? MrTitle { get; internal set; }
        public string Email { get; internal set; }
        public string? CNICNo { get; internal set; }
        public string ContactNumber { get; internal set; }
        public string? CurrAddress { get; internal set; }
        public string? PermAddress { get; internal set; }
        public string? OfficeAddres { get; internal set; }
        public int? CityId { get; internal set; }
        public int? BarCouncilId { get; internal set; }
        public string? BarCouncilNo { get; internal set; }
        public DateTime? EnrollmentDate { get; internal set; }
        public bool? IsContestedCopy { get; internal set; }
        public long UserId { get; internal set; }
    }
    public class LawyerQualificationVM
    {
        public long? QualificationId { get; set; }
        [StringLength(100)]
        public string? DegreeName { get; set; }
        [StringLength(100)]
        public string? InstituteName { get; set; }
        [Required]
        public long UserId { get; set; }
    }
    public class LawyerExpertiesVM
    {
        public long Id { get; set; }
        [Required]
        public long UserId { get; set; }
        [Required]
        public long CategoryId { get; set; }
    }
    public class LawyerUpdateVM
    {
        public LawyerProfileVM LawyerProfile { get; set; } = new LawyerProfileVM();
        public List<LawyerQualificationVM> LawyerQualifications { get; set; } = new List<LawyerQualificationVM>();
        public List<LawyerExpertiesVM> LawyerExperties { get; set; } = new List<LawyerExpertiesVM>();
    }
}
