using Business.ViewModels;
using Data.DomainModels;
using ProjWakalatnama.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public interface ICasesRepository : IBaseRepository<CasesDetail>
    {        
        Task<CourtCases> CreateUpdateCase(CourtCaseVM courtCase);
        Task<CaseDateResponseVM> CreateCaseDate(CaseDateVM detailVM);
        Task<CaseDateVM> UpdateCaseDate(CaseDateVM detailVM);
        Task<CaseDateVM> GetCaseDate(long CaseId, long DateId);
        Task<List<CourtCaseVM>> GetCitizenCases(long? userId);
        Task<List<CourtCaseVM>> GetLawyerCases(long? userId);
        Task<List<CaseDetailVM>> GetCitizenDateList(long? userId,long caseId);
        Task<List<CaseDetailVM>> GetLawyerDateList(long? userId,long caseId);
        Task<CourtCaseVM> GetCaseById(long? caseId);
        Task<CourtCaseDetailsVM> GetCaseDetailsById(long? caseId);
        Task AcceptRejectCaseByLawyer(AcceptRejectCaseVM acceptReject);
        Task<bool> AcceptRejectCase(AcceptRejectCaseVM acceptRejectVm);
        Task<List<CourtCaseVM>> GetCasesForAdminApproval();
        Task<List<CourtCaseVM>> GetCasesForLawyerApproval(long lawywerId);
        Task<bool> AssignEmployeeToCase(CourtCaseVM vm);
        Task<CaseRejectionReason> AddCaseRejectionReason(AcceptRejectCaseVM vm);
        Task<List<CaseRejectionReasonVm>> GetCaseRejectionReason();
        Task<CaseAprovalVM> GetAdminCaseDetails(long CaseId);




        Task<bool> UpdateCaseStatusByAdmin(AdminAcceptRejectCaseVM vM);
        public Task<List<CasesCityWiseReportVm>> GetCityCategoryLegalStatusReport(CasesCityWiseReportVm model);

        public Task<List<CompletePendingCasesCountVm>> GetCompletedPendingCasesCount();

        public Task<List<FinancialTransactionReportVm>> GetFinancialTransactionReport(FinancialReportFilterVm filters);
       
        
        Task<CaseDateResponseVM> CreateCaseDateByAdmin(NewCaseDateVM detailVM);
        Task<NewCaseDateVM> UpdateCaseDateByAdmin(NewCaseDateVM detailVM);
        Task<NewCaseDateVMWithFiles> GetCaseDateByAdmin(long CaseId, long DateId);
        Task<List<GetCourtCaseListVM>> GetAllCasesByAdmin();



        Task<List<CaseDetailVM>> GetDateListByCaseId(long? userId, long caseId);
        Task<bool> DeleteCaseDateByAdmin(long userId, long caseId,long dateId);
        Task<bool> DeleteCaseByAdmin(long userId, long caseId);

        Task<CaseAprovalVM> GetApprovedCaseDetailsByAdmin(long CaseId);
        Task<CourtCases> CreateUpdateCaseByAdmin(AdminCourtCaseVM caseVM);

        Task<List<FetchCaseDateNotificationDataVm>> GetCaseNextDateReminderDataForEmail();
        Task<List<FetchCaseDateNotificationDataVm>> GetCaseNextDateReminderDataForSms();

    }
} 
