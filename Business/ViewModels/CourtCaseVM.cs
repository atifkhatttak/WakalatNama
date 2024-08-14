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
        public string? CaseStatus { get; set; }
        public string? CreatedUserName { get; set; }
        public string? CasePlacedFor { get; set; }
    }

    public class AdminCourtCaseVM
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
        public string? CaseStatus { get; set; }
        public string? CreatedUserName { get; set; }
        public string? CasePlacedFor { get; set; }
    }

    public class CourtCaseDetailsVM
    {
        public string? UserFullName { get; set; }
        public long CaseId { get; set; }
        public long CitizenId { get; set; }
        public long LawyerId { get; set; }
        public string? CaseTitle { get; set; }
        public string? CaseNumber { get; set; }
        public string? CaseDescription { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int CaseStatusId { get; set; }
        public int? CasePlacingId { get; set; }
        public string PartStatus { get; set; }
        public string CaseJurisdiction { get; set; }
        public int CaseFee { get; set; }
        public List<FilesCollectionVM> CaseFiles { get; set; }=new List<FilesCollectionVM>();
        public string CourtLocation { get; internal set; }
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
        public string? CasePlacedFor { get; set; }
        public List<FilesCollectionVM> DateFiles { get; set; } = new List<FilesCollectionVM>();
        public string? CaseDateStatus { get;  set; }
        public DateTime? CreatedDate { get;  set; }
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
        public List<FilesCollectionVM>? DateFiles { get; set; } = new List<FilesCollectionVM>();
    }
    public class NewCaseDateVM
    {
        public long CaseDateId { get; set; }
        public long CaseId { get; set; }
        //public long CitizenId { get; set; }
        //public long LawyerId { get; set; }
        //public long RedundantLawyerId { get; set; }
        [Required(ErrorMessage = "Description is required")]
        public string DateDescription { get; set; }
        public string DateTitle { get; set; }
        [Required(ErrorMessage = "HearingDate is required")]
        public DateTime? HearingDate { get; set; }
        //public IFormFileCollection HearingDocuments { get; set; } = null;
        [Required(ErrorMessage = "Status is required")]
        public int CaseStatusId { get; set; }       
    }
    public class NewCaseDateVMWithFiles:NewCaseDateVM
    {
        public List<FilesCollectionVM> DateFiles { get; set; } = new List<FilesCollectionVM>();
    }
    public class CaseDateResponseVM
    {
        public long HearingDateId { get; set; }
    }
    public class FilesCollectionVM
    {
        public long DocId { get; set; }
        public int? DocType { get; set; }
        public string? DocUrl { get; set; }
        public string? DocName { get; set; }
        public long? DocSize { get; set; }
    }
    public class FileParmsVM
    {
        public long? CaseId { get; set; }
        public long? DateId { get; set; }
        public int DocType { get; set; }
    }
    public class CaseAprovalVM
    {
        public CourtCaseDetailVM CaseDetails { get; set; }
        public CaseCitizenDetailVM CitizenDetail { get; set; }
        public CaseLawyerDetailVM LawyerDetail { get; set; }
        public string PaymentDetail { get; set; }
    }

    public class CourtCaseDetailVM
    {
        public long CaseId { get; internal set; }
        public string? CaseTitle { get; internal set; }
        public string CaseNumber { get; internal set; }
        public int CaseStatusId { get; internal set; }
        public string CaseStatus { get; internal set; }
        public DateTime CreatedDate { get; internal set; }
        public int? CasePlacingId { get; internal set; }
        public string CasePlacing { get; internal set; }
        public int LegalStatusId { get; internal set; }
        public string LegalStatus { get; internal set; }
          public List<FilesCollectionVM> CaseFiles { get; set; } = new List<FilesCollectionVM>();
    }
    public class CaseCitizenDetailVM
    {
        public long UserId { get; internal set; }
        public string Email { get; internal set; }
        public string? FullName { get; internal set; }
        public string? CNICNo { get; internal set; }
        public string? CurrAddress { get; internal set; }
        public string? PermAddress { get; internal set; }
        public string ContactNumber { get; internal set; }
    }
    public class CaseLawyerDetailVM
    {
        public long UserId { get; internal set; }
        public string Email { get; internal set; }
        public string? FullName { get; internal set; }
        public string? CNICNo { get; internal set; }
        public string? CurrAddress { get; internal set; }
        public string? PermAddress { get; internal set; }
        public string ContactNumber { get; internal set; }
        public string? BarCouncilNo { get; internal set; }
    }

    public class AdminAcceptRejectCaseVM
    {
        [Required]
        public long UserId { get; set; }
        [Required]
        public long CitizenId { get; set; }
        [Required]
        public long CaseId { get; set; }
        [Required]
        public long LawyerId { get; set; }
        public long? RedundantLawyerId { get; set; }
        public long? EmployeeId { get; set; }
        [Required]
        public bool CaseStatus { get; set; }
        public int? PaymentStatusId { get; set; }
        public int? RejectionId { get; set; }
        public string? RejectionNote { get; set; }

    }

    public class GetCourtCaseListVM
    {
        public long? CaseId { get; set; }
        public string? CaseDescription { get; set; }
        public string? CaseTitle { get;  set; }
        public long? CitizenId { get;  set; }
        public string? CitizenName { get; internal set; }
        public long? LawyerId { get; internal set; }
        public string? LawyerName { get; internal set; }
        public DateTime? CreatedDate { get; internal set; }
        public int? StatusId { get; internal set; }
        public string? ECaseStatus { get; internal set; }
        public int? LeagalStatusId { get; internal set; }
        public string? LeagalStatus { get; internal set; }
    }
}
