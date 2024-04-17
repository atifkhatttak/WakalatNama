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
        Task<CitizenHomeVM> GetCitizenHome(FilterVM filterVM);
        Task<LawyerVM> GetLawyerProfile(long? LawyerId);
        Task<LawyerUpdateVM> GetLawyerProfileInfo(long? LawyerId);
        Task<CitizenVM> GetCitizenProfile(long? CitizenId);
        Task<LawyerHomeVM> GetLawyerHome(int? lawyerId);
        Task<CitizenVM> CreateCitizenProfile(CitizenVM citizenVM);
        Task<LawyerProfileVM> CreateUpdateLawyerProfile(LawyerProfileVM lawyerVM);
        Task<int> CreateUpdateLawyerExperties(List<LawyerExpertiesVM> expertiesVMs);
        Task<int> CreateUpdateLawyerQaulification(List<LawyerQualificationVM> qualificationVMs);

        Task<List<UserProfileVM>> GetAllUser(bool IsPending, int RoleId);
    }
}
