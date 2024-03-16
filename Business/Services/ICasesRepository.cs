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
        Task<CaseDateVM> CreateUpdateCaseDate(CaseDateVM detailVM);
        Task<List<CourtCaseVM>> GetCitizenCases(long? userId);
        Task<List<CourtCaseVM>> GetLawyerCases(long? userId);
        Task<List<CaseDetailVM>> GetCitizenDateList(long? userId);
        Task<List<CaseDetailVM>> GetLawyerDateList(long? userId);
        Task<CourtCaseVM> GetCaseById(long? caseId);
        Task AcceptRejectCaseByLawyer(AcceptRejectCaseVM acceptReject);
    }
}
