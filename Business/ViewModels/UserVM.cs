using Data.DomainModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ViewModels
{
    public class UserProfileVM: BaseModelVM
    {
        public long ProfileId { get; set; }
        public int RoleId { get; set; }
        public long UserId { get; set; }
        public string? MrTitle { get; set; }
        public string? FullName { get; set; }
        public string? CNICNo { get; set; }
        public string? Gender { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public string? CurrAddress { get; set; }
        public string? PermAddress { get; set; }

        public int? CityId { get; set; }

        public int? CountryId { get; set; }
        public bool? IsOverseas { get; set; }
        public bool? IsForeignQualified { get; set; }
        public string? NICOP { get; set; }
        public string? PassportID { get; set; }
        public int? ResideCountryId { get; set; }
        public string? OverseasContactNo { get; set; }
        public string? OfficeAddress { get; set; }
        public string? LCourtName { get; set; }
        public string? LCourtLocation { get; set; }
        public string? LHighCourtName { get; set; }
        public string? LHighCourtLocation { get; set; }
        public string? Qualification { get; set; }
        public string? Institute { get; set; }
        public int? BarCouncilId { get; set; }
        public string? BarCouncilNo { get; set; }

        public DateTime? EnrollmentDate { get; set; }
        public float? TotalExperience { get; set; }
        public string? AreasOfExpertise { get; set; }
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
        public bool? IsContestedCopy { get; set; }
        public string? ProfileDescription { get; set; }
        public string? FatherName { get; set; }
    }
}
