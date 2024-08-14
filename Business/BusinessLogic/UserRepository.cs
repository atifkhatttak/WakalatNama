using Business.Enums;
using Business.Services;
using Business.ViewModels;
using Data.Context;
using Data.DomainModels;
using Google.Apis.Drive.v3.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProjWakalatnama.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Business.BusinessLogic
{
    public class UserRepository : BaseRepository<UserProfile>, IUserRepository
    {
        private readonly WKNNAMADBCtx ctx;
        private readonly IConfiguration config;
        private readonly ICasesRepository casesRepository;
        private readonly BaseSPRepository baseSP;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly bool isAuthenticated = false;
        private readonly long LoggedInUserId = -1;
        private readonly string LoggedInRole = "";

        public UserRepository(WKNNAMADBCtx ctx, IConfiguration config, IHttpContextAccessor httpContextAccessor) : base(ctx)
        {
            this.ctx = ctx;
            this.config = config;
            this._httpContextAccessor = httpContextAccessor;
            baseSP = new BaseSPRepository(ctx);
            this.isAuthenticated = _httpContextAccessor?.HttpContext?.User.Identity.IsAuthenticated?? false;
            this.LoggedInUserId = isAuthenticated ? Convert.ToInt64(httpContextAccessor.HttpContext.User.FindFirstValue("UserId")) : -1;
        }

        public async Task<CitizenVM> CreateCitizenProfile(CitizenVM citizenVM)
        {
            try
            {

                if (citizenVM != null)
                {
                    if (citizenVM.ProfileId > 0)
                    {
                        var CitizenGet = await ctx.UserProfiles.Where(x => x.ProfileId == citizenVM.ProfileId && x.IsDeleted == false).FirstOrDefaultAsync();

                        if (CitizenGet != null)
                        {
                            CitizenGet.FullName = citizenVM.FullName;
                            CitizenGet.UserName = citizenVM.UserName;
                            CitizenGet.FatherName = citizenVM.FatherName;
                            CitizenGet.CNICNo = citizenVM.CNICNo;
                            CitizenGet.CityId = citizenVM.CityId;
                            CitizenGet.CountryCode = citizenVM.CountryCode;
                            CitizenGet.ContactNumber = citizenVM.ContactNumber;
                            CitizenGet.CurrAddress = citizenVM.CurrAddress;
                            CitizenGet.PermAddress = citizenVM.PermAddress;
                            CitizenGet.ProfilePicUrl = citizenVM.ProfilePic;
                            CitizenGet.CNICFrontUrl = citizenVM.CNICFront;
                            CitizenGet.CNICBackUrl = citizenVM.CNICBack;
                            CitizenGet.IsDeleted = false;
                            CitizenGet.MrTitle = citizenVM.MrTitle;


                            ctx.Entry(CitizenGet).State = EntityState.Modified;
                            await ctx.SaveChangesAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return citizenVM;
        }
        public async Task<CitizenHomeVM> GetCitizenHome(FilterVM filterVM)
        {
            CitizenHomeVM citizenHome = new CitizenHomeVM();
            //List<SqlParameter> sqlParameters= new List<SqlParameter>();
            try
            {

                if (filterVM != null)
                {

                    //if (!isAuthenticated) return CitizenHomeVM;
                    //if (isAuthenticated)
                    //{
                    //    if (LoggedInUserId != filterVM.UserId)
                    //        return CitizenHomeVM;
                    //}



                    SqlParameter[] param = baseSP.CreateSqlParametersFromModel(filterVM);

                    var lawyers = await baseSP.ExecuteStoredProcedureAsync<sp_GetCitizenLawyers_Result>("sp_GetCitizenLawyers", param);
                    //var popularLawyers = await baseSP.ExecuteStoredProcedureAsync<sp_GetCitizenLawyers_Result>("sp_GetCitizenLawyers_Popular", param);

                    if (lawyers != null)
                    {
                        foreach (var item in lawyers)
                        {
                            if (item.Rating > 3)
                            {
                                citizenHome.PopularLawyers.Add(new LawyerVM
                                {
                                    LawyerId = item.UserId,
                                    UserName = item.FullName,
                                    TotalExperience = item.TotalExperience,
                                    Rating = item.Rating,
                                    IsFavourite = item.IsFavourite ?? false,
                                    ProfilePic=item.ProfilePicUrl
                                });
                            }

                            //popular lawyer should also be added into all list
                            citizenHome.Lawyers.Add(new LawyerVM
                            {
                                LawyerId = item.UserId,
                                UserName = item.FullName,
                                TotalExperience = item.TotalExperience,
                                Rating = item.Rating,
                                IsFavourite = item.IsFavourite ?? false,
                                ProfilePic = item.ProfilePicUrl
                            });

                        }

                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return citizenHome;
        }
        public async Task<LawyerVM> GetLawyerProfile(long? LawyerId)
        {
            LawyerVM lawyer = new LawyerVM();
            try
            {

                if (LawyerId > 0)
                {
                    var d = await ctx
                        .UserProfiles
                        .Where(x => x.UserId == LawyerId && x.RoleId == (int)Roles.Lawyer)
                        .FirstOrDefaultAsync();

                    int CasesCount =await ctx
                        .CourtCases
                        .Where(x=>x.LawyerId==LawyerId && !x.IsDeleted)
                        .CountAsync();

                    var favourit=await ctx
                            .Favourites
                            .Where(x => x.LawyerId == LawyerId && x.UserId == LoggedInUserId && !x.IsDeleted)
                            .FirstOrDefaultAsync();

                    int TotalClient = 0;
                    if (d != null)
                    {
                        lawyer.Id = d.UserId;
                        lawyer.UserName = d.FullName;
                        lawyer.ProfilePic = d.ProfilePicUrl;
                        lawyer.ProfileDescription = d.ProfileDescription!;
                        lawyer.TotalExperience = d.TotalExperience;
                        lawyer.Rating = d.Rating;
                        lawyer.IsFavourite = favourit?.IsFavourite??false;
                        lawyer.CompletedCase = CasesCount;
                        lawyer.TotalClient = TotalClient;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lawyer;
        }
        public async Task<LawyerHomeVM> GetLawyerHome(int? lawyerId)
        {
            LawyerHomeVM lawyerHome = new LawyerHomeVM();
            try
            {

                if (lawyerId > 0)
                {
                    lawyerHome.TotalCases = await ctx.CourtCases.CountAsync(x => x.LawyerId == lawyerId && x.StatusId==(int)ECaseStatuses.Approved && !x.IsDeleted);
                    lawyerHome.CompltedCase = await ctx.CourtCases.CountAsync(x => x.LawyerId == lawyerId && x.LegalStatusId == (int)ELeagalStatus.JudgementAndDecreeReservedOrAnnounced && !x.IsDeleted);


                    lawyerHome.CourtCases = await (from c in ctx.CourtCases
                                                   join u in ctx.UserProfiles on c.CitizenId equals u.UserId
                                                   join cat in ctx.CaseCategories on c.CategoryId equals cat.ID
                                                   where c.LawyerId == lawyerId && u.RoleId == (int)Roles.Citizen
                                                   && (c.StatusId == (int)ECaseStatuses.AdminAccepted || c.StatusId == (int)ECaseStatuses.ForwardedToLawyer) && !(u.IsDeleted && c.IsDeleted)
                                                   select new CourtCaseVM
                                                   {
                                                       UserFullName = u.FullName,
                                                       CaseId = c.CaseId,
                                                       CitizenId = c.CitizenId,
                                                       LawyerId = c.LawyerId,
                                                       CaseNumber = c.CaseNumber,
                                                       CaseTitle = c.CaseTitle,
                                                       CategoryId = c.CategoryId,
                                                       CategoryName = cat.CategoryName,
                                                       CaseStatusId = c.LegalStatusId
                                                   })
                          .ToListAsync();

                    //if (caseList.Any())
                    //{
                    //    userCases = new List<CourtCaseVM>();
                    //    foreach (var item in caseList)
                    //    {
                    //        userCases.Add(new CourtCaseVM
                    //        {
                    //            UserFullName = item.FullName,

                    //            CaseId = item.CaseId,
                    //            CitizenId = item.CitizenId,
                    //            LawyerId = item.LawyerId,
                    //            CaseNumber = item.CaseNumber,
                    //            CaseTitle = item.CaseTitle,
                    //            CategoryId = item.CategoryId,
                    //            CategoryName = item.CategoryName
                    //        });
                    //    }
                    //}

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lawyerHome;
        }
        public async Task<CitizenVM> GetCitizenProfile(long? CitizenId)
        {
            CitizenVM citizenVM = new CitizenVM();
            try
            {
                //if (!isAuthenticated) return citizenVM;
                //if (isAuthenticated)
                //{
                //    if (LoggedInUserId != CitizenId)
                //        return citizenVM;
                //}

                if (CitizenId > 0)
                {
                    var d = await ctx
                        .UserProfiles
                        .Where(x => x.UserId == CitizenId
                    && (x.RoleId == (int)Roles.Citizen && x.IsActive == true && x.IsVerified == true && !x.IsDeleted))
                        .FirstOrDefaultAsync();

                    //string ProfilePic = ctx.UserDocuments.Where(x => x.UserId == CitizenId && !x.IsDeleted).FirstOrDefault()?.DocPath ?? "";

                    if (d != null)
                    {
                        citizenVM.UserId = d.UserId;
                        citizenVM.UserName = d.UserName;
                        citizenVM.ProfileId = d.ProfileId;
                        citizenVM.ProfilePic = d.ProfilePicUrl;
                        citizenVM.CNICFront = d.CNICFrontUrl;
                        citizenVM.CNICBack = d.CNICBackUrl;
                        citizenVM.FullName = d.FullName;
                        citizenVM.FatherName = d.FatherName;
                        citizenVM.MrTitle = d.MrTitle;
                        citizenVM.Email = d.Email;
                        citizenVM.CNICNo = d.CNICNo;
                        citizenVM.ContactNumber = d.ContactNumber;
                        citizenVM.CountryCode = d.CountryCode;
                        citizenVM.CurrAddress = d.CurrAddress;
                        citizenVM.PermAddress = d.PermAddress;
                        citizenVM.CityId=d.CityId;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return citizenVM;
        }
        public async Task<LawyerProfileVM> CreateUpdateLawyerProfile(LawyerProfileVM lawyerVM)
        {
            try
            {

                if (lawyerVM != null && lawyerVM.ProfileId > 0)
                {
                    var GetLawyer = await ctx.UserProfiles.Where(x => x.ProfileId == lawyerVM.ProfileId && !x.IsDeleted).FirstOrDefaultAsync();

                    if (GetLawyer != null)
                    {
                        GetLawyer.MrTitle = lawyerVM.MrTitle;
                        GetLawyer.FullName = lawyerVM.FullName;
                        //GetLawyer.Email = lawyerVM.Email;
                        GetLawyer.CNICNo = lawyerVM.CNICNo;
                        //GetLawyer.ContactNumber = lawyerVM.ContactNumber;
                        GetLawyer.CurrAddress = lawyerVM.CurrAddress;
                        GetLawyer.PermAddress = lawyerVM.PermAddress;
                        GetLawyer.OfficeAddress = lawyerVM.OfficeAddres;
                        GetLawyer.CityId = lawyerVM.CityId;
                        GetLawyer.BarCouncilId = lawyerVM.BarCouncilId;
                        GetLawyer.BarCouncilNo = lawyerVM.BarCouncilNo;
                        GetLawyer.EnrollmentDate = lawyerVM.EnrollmentDate;
                        GetLawyer.IsContestedCopy = lawyerVM.IsContestedCopy;
                        GetLawyer.ProfileDescription = lawyerVM.ProfileDescription;
                        GetLawyer.ProfilePicUrl = lawyerVM.ProfilePic;
                        GetLawyer.CNICFrontUrl = lawyerVM.CNICFrontUrl;
                        GetLawyer.CNICBackUrl = lawyerVM.CNICBackUrl;
                        GetLawyer.BarCouncilFrontUrl = lawyerVM.BarCouncilFrontUrl;
                        GetLawyer.BarCouncilBackUrl = lawyerVM.BarCouncilBackUrl;
                        GetLawyer.CountryCode = lawyerVM.CountryCode;

                        ctx.Entry(GetLawyer).State = EntityState.Modified;
                        await ctx.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lawyerVM;
        }

        public async Task<int> CreateUpdateLawyerExperties(List<LawyerExpertiesVM> expertiesVMs)
        {
            int result = 0;
            try
            {
                if (expertiesVMs != null && expertiesVMs.Any())
                {
                    List<LawyerExperties> news = new List<LawyerExperties>();
                    List<LawyerExperties> updates = new List<LawyerExperties>();
                    foreach (var item in expertiesVMs)
                    {
                        if (item.Id == 0)
                        {
                            news.Add(new LawyerExperties()
                            {
                                UserId = item.UserId,
                                CategoryId = item.CategoryId
                            });
                        }
                        if (item.Id > 0)
                        {
                            var exp = await ctx.LawyerExperties.FindAsync(item.Id);
                            if (exp != null)
                            {
                                exp.UserId = item.UserId;
                                exp.CategoryId = item.CategoryId;
                                updates.Add(exp);
                            }
                        }
                    }

                    ctx.UpdateRange(updates);
                    await ctx.AddRangeAsync(news);
                    result = await ctx.SaveChangesAsync();

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return result;
        }

        public async Task<int> CreateUpdateLawyerQaulification(List<LawyerQualificationVM> qualificationVMs)
        {
            int result = 0;
            try
            {
                if (qualificationVMs != null && qualificationVMs.Any())
                {
                    List<LawyerQualification> news = new List<LawyerQualification>();
                    List<LawyerQualification> updates = new List<LawyerQualification>();
                    foreach (var item in qualificationVMs)
                    {
                        if (item.QualificationId == 0)
                        {
                            news.Add(new LawyerQualification()
                            {
                                UserId = item.UserId,
                                DegreeName = item.DegreeName,
                                InstituteName = item.InstituteName
                            });
                        }
                        if (item.QualificationId > 0)
                        {
                            var exp = await ctx.LawyerQualifications.FindAsync(item.QualificationId);
                            if (exp != null)
                            {
                                exp.DegreeName = item.DegreeName;
                                exp.InstituteName = item.InstituteName;
                                updates.Add(exp);
                            }
                        }
                    }

                    ctx.UpdateRange(updates);//bulk update
                    await ctx.AddRangeAsync(news);//bulk insert
                    result = await ctx.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return result;
        }

        public async Task<LawyerUpdateVM> GetLawyerProfileInfo(long? LawyerId)
        {
            LawyerUpdateVM lawyer = new LawyerUpdateVM();
            try
            {
                //if (!isAuthenticated) return lawyer;
                //if (isAuthenticated)
                //{
                //    if (LoggedInUserId != LawyerId)
                //        return lawyer;
                //}

                if (LawyerId > 0)
                {
                    var d = await ctx.UserProfiles.FirstOrDefaultAsync(x => x.UserId == LawyerId && x.RoleId == (int)Roles.Lawyer && !x.IsDeleted);

                    if (d != null)
                    {

                        lawyer.LawyerProfile.UserId = d.UserId;
                        lawyer.LawyerProfile.ProfileId = d.ProfileId;
                        lawyer.LawyerProfile.ProfilePic = d.ProfilePicUrl;
                        lawyer.LawyerProfile.CNICFrontUrl = d.CNICFrontUrl;
                        lawyer.LawyerProfile.CNICBackUrl = d.CNICBackUrl;
                        lawyer.LawyerProfile.BarCouncilFrontUrl = d.BarCouncilFrontUrl;
                        lawyer.LawyerProfile.BarCouncilBackUrl = d.BarCouncilBackUrl;
                        lawyer.LawyerProfile.MrTitle = d.MrTitle;
                        lawyer.LawyerProfile.FullName = d.FullName;
                        lawyer.LawyerProfile.Email = d.Email;
                        lawyer.LawyerProfile.CNICNo = d.CNICNo;
                        lawyer.LawyerProfile.ContactNumber = d.ContactNumber;
                        lawyer.LawyerProfile.CurrAddress = d.CurrAddress;
                        lawyer.LawyerProfile.PermAddress = d.PermAddress;
                        lawyer.LawyerProfile.OfficeAddres = d.OfficeAddress;
                        lawyer.LawyerProfile.CityId = d.CityId;
                        lawyer.LawyerProfile.BarCouncilId = d.BarCouncilId;
                        lawyer.LawyerProfile.BarCouncilNo = d.BarCouncilNo;
                        lawyer.LawyerProfile.EnrollmentDate = d.EnrollmentDate;
                        lawyer.LawyerProfile.IsContestedCopy = d.IsContestedCopy;
                        lawyer.LawyerProfile.FatherName = d.FatherName;
                        lawyer.LawyerProfile.CountryCode = d.CountryCode;
                    }

                    lawyer.LawyerExperties = await ctx.LawyerExperties.Where(x => x.UserId == LawyerId).Select(s => new LawyerExpertiesVM()
                    {
                        Id = s.Id,
                        UserId = s.UserId,
                        CategoryId = s.CategoryId
                    }).ToListAsync();

                    lawyer.LawyerQualifications = await ctx.LawyerQualifications.Where(x => x.UserId == LawyerId).Select(s => new LawyerQualificationVM()
                    {
                        QualificationId = s.Id,
                        DegreeName = s.DegreeName,
                        InstituteName = s.InstituteName
                    }).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lawyer;
        }

        public async Task<LawyerUpdateVM> GetLawyerProfileInfoForAdmin(long? LawyerId)
        {
            LawyerUpdateVM lawyer = new LawyerUpdateVM();
            try
            {
                //if (!isAuthenticated) return lawyer;
                //if (isAuthenticated)
                //{
                //    if (LoggedInUserId != LawyerId)
                //        return lawyer;
                //}

                if (LawyerId > 0)
                {
                    var d = await ctx
                        .UserProfiles
                        .FirstOrDefaultAsync(x => x.UserId == LawyerId && x.RoleId == (int)Roles.Lawyer && x.IsVerified== false && !x.IsDeleted);

                    if (d != null)
                    {

                        lawyer.LawyerProfile.UserId = d.UserId;
                        lawyer.LawyerProfile.ProfileId = d.ProfileId;
                        lawyer.LawyerProfile.ProfilePic = d.ProfilePicUrl;
                        lawyer.LawyerProfile.MrTitle = d.MrTitle;
                        lawyer.LawyerProfile.FullName = d.FullName;
                        lawyer.LawyerProfile.Email = d.Email;
                        lawyer.LawyerProfile.CNICNo = d.CNICNo;
                        lawyer.LawyerProfile.ContactNumber = d.ContactNumber;
                        lawyer.LawyerProfile.CurrAddress = d.CurrAddress;
                        lawyer.LawyerProfile.PermAddress = d.PermAddress;
                        lawyer.LawyerProfile.OfficeAddres = d.OfficeAddress;
                        lawyer.LawyerProfile.CityId = d.CityId;
                        lawyer.LawyerProfile.BarCouncilId = d.BarCouncilId;
                        lawyer.LawyerProfile.BarCouncilNo = d.BarCouncilNo;
                        lawyer.LawyerProfile.EnrollmentDate = d.EnrollmentDate;
                        lawyer.LawyerProfile.IsContestedCopy = d.IsContestedCopy;
                    }

                    lawyer.LawyerExperties = await ctx.LawyerExperties.Where(x => x.UserId == LawyerId).Select(s => new LawyerExpertiesVM()
                    {
                        Id = s.Id,
                        UserId = s.UserId,
                        CategoryId = s.CategoryId
                    }).ToListAsync();

                    lawyer.LawyerQualifications = await ctx.LawyerQualifications.Where(x => x.UserId == LawyerId).Select(s => new LawyerQualificationVM()
                    {
                        QualificationId = s.Id,
                        DegreeName = s.DegreeName,
                        InstituteName = s.InstituteName
                    }).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lawyer;
        }

        public async Task<List<UserProfileVM>> GetAllUser(bool IsPending, int RoleId)
        {
            List<UserProfileVM> AllUsers = new List<UserProfileVM>();
            try
            {
                //if (!isAuthenticated) return lawyer;
                //if (isAuthenticated)
                //{
                //    if (LoggedInUserId != LawyerId)
                //        return lawyer;
                //}

                AllUsers = await ctx.UserProfiles
     .Where(x => !x.IsDeleted && (!IsPending || x.IsVerified == false) && (RoleId == 0 || x.RoleId == RoleId))
    .Select(x => new UserProfileVM
    {
        ProfileId = x.ProfileId,
        RoleId = x.RoleId,
        UserId = x.UserId,
        MrTitle = x.MrTitle,
        FullName = x.FullName,
        CNICNo = x.CNICNo,
        Gender = x.Gender,
        Email = x.Email,
        ContactNumber = x.ContactNumber,
        CurrAddress = x.CurrAddress,
        PermAddress = x.PermAddress,
        CityId = x.CityId,
        CountryId = x.CountryId,
        IsOverseas = x.IsOverseas,
        IsForeignQualified = x.IsForeignQualified,
        NICOP = x.NICOP,
        PassportID = x.PassportID,
        ResideCountryId = x.ResideCountryId,
        OverseasContactNo = x.OverseasContactNo,
        OfficeAddress = x.OfficeAddress,
        LCourtName = x.LCourtName,
        LCourtLocation = x.LCourtLocation,
        LHighCourtName = x.LHighCourtName,
        LHighCourtLocation = x.LHighCourtLocation,
        Qualification = x.Qualification,
        Institute = x.Institute,
        BarCouncilId = x.BarCouncilId,
        BarCouncilNo = x.BarCouncilNo,
        EnrollmentDate = x.EnrollmentDate,
        TotalExperience = x.TotalExperience,
        AreasOfExpertise = x.AreasOfExpertise,
        IsAlert = x.IsAlert,
        IsSMS = x.IsSMS,
        IsEmail = x.IsEmail,
        IsPushAlert = x.IsPushAlert,
        IsCreateMeeting = x.IsCreateMeeting,
        IsAgreed = x.IsAgreed,
        IsActive = x.IsActive,
        IsVerified = x.IsVerified,
        Rating = x.Rating,
        IsFavourite = x.IsFavourite,
        IsContestedCopy = x.IsContestedCopy,
        ProfileDescription = x.ProfileDescription,
        FatherName = x.FatherName

    })
    .ToListAsync();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return AllUsers;
        }

        public async Task<bool> AcceptRejectLawyerByAdmin(AcceptLawyer vm)
        {
            int result = 0;
            try
            {
                if (vm!=null)
                {
                    var GetLawyer = await ctx
                        .UserProfiles
                        .Where(x => x.UserId == vm.UserId && x.RoleId==(int)Roles.Lawyer && x.IsVerified == false && !x.IsDeleted)
                        .FirstOrDefaultAsync();

                    if (GetLawyer!=null)
                    {

                        GetLawyer.IsActive= vm.IsApproved;
                        GetLawyer.IsAgreed= vm.IsApproved;
                        GetLawyer.IsVerified= vm.IsApproved;
                        GetLawyer.UpdateDate = DateTime.Now;
                        GetLawyer.UpdatedBy = vm.LoggedInUserId;

                        ctx.Entry(GetLawyer).State = EntityState.Modified;
                        result= await ctx.SaveChangesAsync();
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return result!=0;
        }

        public async Task<bool> DeleteUser(long LoggedInUser, long UserId)
        {
            //if (!isAuthenticated) return false;
            //if (isAuthenticated)
            //{
            //    if (LoggedInUserId != LoggedInUser)
            //        return false;
            //}

            var profile = await ctx.UserProfiles.Where(x => x.UserId == UserId && !x.IsDeleted).FirstOrDefaultAsync();
            var user = await ctx.Users.Where(x => x.Id == UserId && !x.IsDeleted).FirstOrDefaultAsync();
            profile.IsDeleted=user.IsDeleted = true;
            profile.IsActive = false;


            return await ctx.SaveChangesAsync()!=0;
        }

       

        public async Task<List<UserCountByRoleVm>> UserCountByRoleType(int RoleType = -1)
        {
           var userCountByRole = await ctx.Database.SqlQueryRaw<UserCountByRoleVm>($@" 
                                       SELECT Name,
                                              COUNT(1) AS Count
                                       FROM dbo.UserProfiles
                                           INNER JOIN dbo.AspNetRoles
                                               ON RoleId = Id
                                       WHERE @RoleID = -1
                                             OR RoleId = @RoleID
                                       GROUP BY RoleId,
                                                Name
", new SqlParameter("@RoleID", RoleType))
                                 .ToListAsync();
            return userCountByRole;
        }

        public async Task<List<UserApprovedUnApprovedCountVm>> UserCountByApprovedUnApprovedStatus(int RoleType = -1)
        { 
            var userCountByRole = await ctx.Database.SqlQueryRaw<UserApprovedUnApprovedCountVm>($@"  
                                       SELECT Name,
                                            COUNT(1) AS Count,CASE WHEN IsVerified=1 then 'Approved' ELSE 'Pending' END AS Status
                                     FROM dbo.UserProfiles
                                         INNER JOIN dbo.AspNetRoles 
                                             ON RoleId = Id
                                     WHERE (@RoleID = -1
                                           OR RoleId = @RoleID)  
                                     GROUP BY RoleId, IsVerified,
                                              Name
", new SqlParameter("@RoleID", RoleType))
                                  .ToListAsync();
            return userCountByRole;
        }

        public async Task<bool> CreateDiaryReminder(string text, DateTime reminderDate)
        {
            return true;
        }


    }
}
