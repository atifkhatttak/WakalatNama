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

        public UserRepository(WKNNAMADBCtx ctx, IConfiguration config,IHttpContextAccessor httpContextAccessor) : base(ctx)
        {
            this.ctx = ctx;
            this.config = config;
            this._httpContextAccessor=httpContextAccessor;
            baseSP = new BaseSPRepository(ctx);
            this.isAuthenticated = _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
            //this.LoggedInUserId = isAuthenticated ? Convert.ToInt64(_httpContextAccessor.HttpContext.User.FindFirstValue("UserId")) : -1;
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
                            CitizenGet.FatherName = citizenVM.FatherName;
                            CitizenGet.CNICNo = citizenVM.CNICNo;
                            CitizenGet.CityId = citizenVM.CityId;
                            //CitizenGet.CountryCode = citizenVM.CountryCode;
                            CitizenGet.ContactNumber = citizenVM.ContactNumber;
                            CitizenGet.CurrAddress = citizenVM.CurrAddress;
                            CitizenGet.PermAddress = citizenVM.PermAddress;
                            CitizenGet.IsDeleted = false;
                            //if (citizenVM.ProfilePhoto!=null)
                            //{
                            //    UserDocument document = new UserDocument();
                            //    document.UserId = citizenVM.UserId;
                            //   document.DocName=citizenVM.ProfilePhoto.Name;
                            //}
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
                                    IsFavourite = item.IsFavourite ?? false
                                });
                            }

                            //popular lawyer should also be added into all list
                                citizenHome.Lawyers.Add(new LawyerVM
                                {
                                    LawyerId = item.UserId,
                                    UserName = item.FullName,
                                    TotalExperience = item.TotalExperience,
                                    Rating = item.Rating,
                                    IsFavourite = item.IsFavourite ?? false
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
                    var d = await ctx.UserProfiles.Where(x => x.UserId== LawyerId && x.RoleId==(int)Roles.Lawyer).FirstOrDefaultAsync();
                    int CasesCount=0;
                    int TotalClient = 0;
                    if (d != null)
                    {
                        lawyer.Id = d.UserId;
                        lawyer.UserName = d.FullName;
                        lawyer.ProfilePic = "";
                        lawyer.ProfileDescription = d.ProfileDescription!;
                        lawyer.TotalExperience = d.TotalExperience;
                        lawyer.Rating = d.Rating;
                        lawyer.IsFavourite = d.IsFavourite;
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
                    lawyerHome.TotalCases = await ctx.CourtCases.CountAsync(x => x.LawyerId== lawyerId && x.IsDeleted==false);
                    lawyerHome.CompltedCase = await ctx.CourtCases.CountAsync(x => x.LawyerId== lawyerId && x.LegalStatusId ==(int)CaseLegalStatus.Initiated && x.IsDeleted==false);


                    lawyerHome.CourtCases = await (from c in ctx.CourtCases 
                                              join u in ctx.UserProfiles on c.CitizenId equals u.UserId
                                              join cat in ctx.CaseCategories on c.CategoryId equals cat.ID
                                              where c.LawyerId == lawyerId && u.RoleId ==(int)Roles.Citizen && (u.IsDeleted == false && c.IsDeleted == false)
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
                                                  CaseStatusId=c.LegalStatusId
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
            CitizenVM citizenVM= new CitizenVM();
            try
            {

                if (CitizenId > 0)
                {
                    var d = await ctx
                        .UserProfiles
                        .Where(x => x.UserId == CitizenId
                    && (x.RoleId == (int)Roles.Citizen && x.IsActive == true && x.IsVerified == true && x.IsDeleted == false))
                        .FirstOrDefaultAsync();

                    if (d != null)
                    {
                        citizenVM.UserId = d.UserId;
                        citizenVM.ProfileId = d.ProfileId;
                        citizenVM.ProfilePic = "";
                        citizenVM.FullName = d.FullName;
                        citizenVM.FatherName = d.FatherName;
                        citizenVM.Email = d.Email;
                        citizenVM.CNICNo = d.CNICNo;
                        citizenVM.ContactNumber = d.ContactNumber;
                        citizenVM.CurrAddress = d.CurrAddress;
                        citizenVM.PermAddress = d.PermAddress;
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
                            GetLawyer.Email = lawyerVM.Email;
                            GetLawyer.CNICNo = lawyerVM.CNICNo;                            
                            GetLawyer.ContactNumber = lawyerVM.ContactNumber;
                            GetLawyer.CurrAddress = lawyerVM.CurrAddress;
                            GetLawyer.PermAddress = lawyerVM.PermAddress;
                            GetLawyer.OfficeAddress = lawyerVM.OfficeAddres;
                            GetLawyer.CityId = lawyerVM.CityId;
                            GetLawyer.BarCouncilId = lawyerVM.BarCouncilId;
                            GetLawyer.BarCouncilNo = lawyerVM.BarCouncilNo;
                            GetLawyer.EnrollmentDate = lawyerVM.EnrollmentDate;
                            GetLawyer.IsContestedCopy = lawyerVM.IsContestedCopy;
                         
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
                if (expertiesVMs!=null && expertiesVMs.Any())
                {
                    List<LawyerExperties> news = new List<LawyerExperties>();
                    List<LawyerExperties> updates = new List<LawyerExperties>();
                    foreach (var item in expertiesVMs)
                    {
                        if (item.Id==0)
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
                            if (exp != null) {
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
                            var exp =await ctx.LawyerQualifications.FindAsync(item.QualificationId);
                            if (exp != null)
                            {
                                exp.DegreeName = item.DegreeName;
                                exp.InstituteName = item.InstituteName;
                                updates.Add(exp);
                            }
                        }
                    }

                   ctx.UpdateRange(updates);//bulk update
                  await  ctx.AddRangeAsync(news);//bulk insert
                  result=  await ctx.SaveChangesAsync();
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

                if (LawyerId > 0)
                {
                    var d = await ctx.UserProfiles.Where(x => x.UserId == LawyerId && x.RoleId == (int)Roles.Lawyer && !x.IsDeleted).FirstOrDefaultAsync();
                   
                    if (d != null)
                    {
                        lawyer.LawyerProfile.UserId = d.UserId;
                        lawyer.LawyerProfile.ProfileId=d.ProfileId;
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
                        Id=s.Id,
                        UserId=s.UserId,
                        CategoryId=s.CategoryId
                    }).ToListAsync();

                    lawyer.LawyerQualifications = await ctx.LawyerQualifications.Where(x => x.UserId == LawyerId).Select(s => new LawyerQualificationVM()
                    {
                        QualificationId = s.Id,
                        DegreeName=s.DegreeName,
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
    }
}
