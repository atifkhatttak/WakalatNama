using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Data.DomainModels;

namespace ProjWakalatnama.DataLayer.Models
{
    public class UserProfile : BaseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ProfileId { get; set; }

        [Required]
        public int RoleId { get; set; }

        [Required]
        public long UserId { get; set; }
        public string? MrTitle { get; set; }

        [MaxLength(100)]
        public string? FullName { get; set; }

        [StringLength(15)]
        public string? CNICNo { get; set; }

        //[StringLength(255)]
        //public string CNICPicFront { get; set; }

        //[StringLength(255)]
        //public string CNICPicBack { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }

        [StringLength(255)]
        public string Email { get; set; }

        [StringLength(20)]
        public string ContactNumber { get; set; }
        //public string? CountryCode { get; set; }

        [MaxLength]
        public string? CurrAddress { get; set; }

        [MaxLength]
        public string? PermAddress { get; set; }

        public int? CityId { get; set; }

        public int? CountryId { get; set; }

        //[StringLength(255)]
        //public string ProfilePic { get; set; }
        [DefaultValue(false)]
        public bool? IsOverseas { get; set; }
        [DefaultValue(false)]
        public bool? IsForeignQualified { get; set; }

        [StringLength(50)]
        public string? NICOP { get; set; }

        //[StringLength(250)]
        //public string NICOPPic { get; set; }

        [StringLength(15)]
        public string? PassportID { get; set; }

        //[StringLength(250)]
        //public string PassportPic { get; set; }

        public int? ResideCountryId { get; set; }

        [StringLength(20)]
        public string? OverseasContactNo { get; set; }

        [MaxLength]
        public string? OfficeAddress { get; set; }

        [MaxLength(255)]
        public string? LCourtName { get; set; }

        [MaxLength]
        public string? LCourtLocation { get; set; }

        [MaxLength(255)]
        public string? LHighCourtName { get; set; }

        [MaxLength]
        public string? LHighCourtLocation { get; set; }

        [StringLength(100)]
        public string? Qualification { get; set; }

        [StringLength(255)]
        public string? Institute { get; set; }
        public int? BarCouncilId { get; set; }
        [StringLength(255)]
        public string? BarCouncilNo { get; set; }

        public DateTime? EnrollmentDate { get; set; }

        //[StringLength(255)]
        //public string BarCouncilCardScanFront { get; set; }

        //[StringLength(255)]
        //public string BarCouncilCardScanBack { get; set; }
        public float? TotalExperience { get; set;}
        [StringLength(255)]
        public string? AreasOfExpertise { get; set; }

        [MaxLength]
        public string? AreasOfExpertiseOrther { get; set; }

        public bool? IsAlert { get; set; }

        public bool? IsSMS { get; set; }

        public bool? IsEmail { get; set; }

        public bool? IsPushAlert { get; set; }

        public bool? IsCreateMeeting { get; set; }

        public bool? IsAgreed { get; set; }

        public bool? IsActive { get; set; }

        public bool? IsVerified { get; set; }

        public float? Rating { get; set; }
        public bool? IsFavourite { get; set; }
        [DefaultValue(false)]
        public bool? IsContestedCopy { get; set; }
        public string? ProfileDescription { get; set; }
        public string? FatherName { get; set; }
        public string? ProfilePicUrl { get; set; }
        public string? CNICFrontUrl { get; set; }
        public string? CNICBackUrl { get; set; }
        public string? CountryCode { get; set; }
        public string? UserName { get; set; }
        public string? BarCouncilFrontUrl { get; set; }
        public string? BarCouncilBackUrl { get; set; }
        public string? LawyerCode { get; set; }
        public string? DeviceToken { get; set; }
    }
}