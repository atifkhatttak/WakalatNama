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
        Task<LawyerUpdateVM> GetLawyerProfileInfoForAdmin(long? LawyerId);
        Task<CitizenVM> GetCitizenProfile(long? CitizenId);
        Task<LawyerHomeVM> GetLawyerHome(int? lawyerId);
        Task<CitizenVM> CreateCitizenProfile(CitizenVM citizenVM);
        Task<LawyerProfileVM> CreateUpdateLawyerProfile(LawyerProfileVM lawyerVM);
        Task<int> CreateUpdateLawyerExperties(List<LawyerExpertiesVM> expertiesVMs);
        Task<int> CreateUpdateLawyerQaulification(List<LawyerQualificationVM> qualificationVMs);

        Task<List<UserProfileVM>> GetAllUser(bool IsPending, int RoleId);
        Task<bool> AcceptRejectLawyerByAdmin(AcceptLawyer vm);
        Task<bool> DeleteUser(long LoggedInUser,long UserId);
         
        Task<List<UserCountByRoleVm>> UserCountByRoleType(int RoleType=-1);
        Task<List<UserApprovedUnApprovedCountVm>> UserCountByApprovedUnApprovedStatus(int RoleType = -1);

        Task<bool> CreateDiaryReminder(string text,DateTime reminderDate);
    }
}
