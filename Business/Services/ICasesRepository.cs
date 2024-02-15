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
        Task CreateCase(CourtCaseVM courtCase);
        Task<List<CourtCaseVM>> GetUserCases(long? userId);
        Task<List<CaseDetailVM>> GetUserDateList(long? userId);
    }
}
