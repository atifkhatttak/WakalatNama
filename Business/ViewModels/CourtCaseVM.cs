using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Data.DomainModels;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Business.ViewModels
{
    public class CourtCaseVM
    {
        public long? CaseId { get; set; }
        [Required]
        public long CitizenId { get; set; }
        public long LawyerId { get; set; }
        public long? RedundantLawyerId { get; set; }
        public string? CaseNumber { get; set; }
        [Required]
        public int? PartyId { get; set; }
        [Required]
        public int? CategoryId { get; set; }
        [Required]
        public int? CaseNatureId { get; set; }
        [Required]
        public string? CaseDescription { get; set; }
        [Required]
        public int? CaseJurisdictionId { get; set; }
        [Required]
        public int CourtId { get; set; }
        [Required]
        public int? CasePlacingId { get; set; }
        public IFormFileCollection? Documents { get; set; } = null;
        public DateTime? CreatedDate { get; set; }

        public long? CreatedUser { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public long? ModifiedUser { get; set; }
        public bool? IsDeleted { get; set; }
        public string? CaseTitle { get; set; }
        public long? UserId { get; set; }
        public string? UserFullName { get; set; }
        public string? CategoryName { get; set; }
        public int? CaseStatusId { get; set; }
        public long? AssignEmployeeId { set; get; }
    }
    public class CaseDetailVM
    {
        public long? HearingDateId { get; set; }
        public long CaseId { get; set; }
        public long CitizenId { get; set; }
        public long LawyerId { get; set; }
        public long RedundantLawyerId { get; set; }
        public string CaseNumber { get; set; }

        public int? PartyId { get; set; }

        public int? CategoryId { get; set; }
        [Required(ErrorMessage = "Description is required")]
        public string DateDescription { get; set; }
        public string DateTitle { get; set; }
        [Required(ErrorMessage ="HearingDate is required")]
        public DateTime? HearingDate { get; set; }
        public IFormFileCollection HearingDocuments { get; set; } = null;
        [Required(ErrorMessage = "Status is required")]
        public int CaseStatusId { get; set; }
        public string CaseTitle { get; set; }
        public string DocName { get; set; }
        public long? CaseDetailId { get; set; }
    }
    public class CaseDateVM
    {
        public long CaseDateId { get; set; }
        public long CaseId { get; set; }
        public long CitizenId { get; set; }
        public long LawyerId { get; set; }
        public long RedundantLawyerId { get; set; }
        [Required(ErrorMessage = "Description is required")]
        public string DateDescription { get; set; }
        public string DateTitle { get; set; }
        [Required(ErrorMessage = "HearingDate is required")]
        public DateTime? HearingDate { get; set; }
        //public IFormFileCollection HearingDocuments { get; set; } = null;
        [Required(ErrorMessage = "Status is required")]
        public int CaseStatusId { get; set; }
    }
}
