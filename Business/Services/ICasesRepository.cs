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
    public interface ICasesRepository : IBaseRepository<CourtCases>
    {
        Task CreateCase(CourtCaseVM courtCase);
    }
}
