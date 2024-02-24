using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ProjWakalatnama.DataLayer.Models
{
    public class UserProfile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ProfileId { get; set; }

        [Required]
        public int RoleId { get; set; }

        [Required]
        public long UserId { get; set; }

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

        public bool? IsOverseas { get; set; }

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

        [StringLength(100)]
        public string? BarCouncil { get; set; }

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
        public DateTime? CreatedDate { get; set; }

        public long? CreatedUser { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public long? ModifiedUser { get; set; }

        public bool? IsActive { get; set; }

        public bool? IsVerified { get; set; }

        [DefaultValue(false)]
        public bool? IsDeleted { get; set; }
        public float? Rating { get; set; }
        public bool? IsFavourite { get; set; }
        public string? ProfileDescription { get; set; }
        public string? FatherName { get; set; }
    }
}