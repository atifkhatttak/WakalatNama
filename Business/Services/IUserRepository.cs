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
    public interface IUserRepository : IBaseRepository<UserProfile>
    {
        Task<List<LawyerVM>> GetLawyerList(FilterVM filterVM);
        Task<LawyerVM> GetLawyerProfile(long? LawyerId);
        Task<CitizenVM> GetCitizenProfile(long? CitizenId);
        Task<LawyerHomeVM> GetLawyerHome(int? lawyerId);
        Task<CitizenVM> CreateCitizenProfile(CitizenVM citizenVM);
    }
}
