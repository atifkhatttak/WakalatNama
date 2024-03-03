using Business.Enums;
using Business.Services;
using Business.ViewModels;
using Data.Context;
using Data.DomainModels;
using Google.Apis.Drive.v3.Data;
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

        public UserRepository(WKNNAMADBCtx ctx, IConfiguration config) : base(ctx)
        {
            this.ctx = ctx;
            this.config = config;
            baseSP = new BaseSPRepository(ctx);
        }

        public async Task<CitizenVM> CreateCitizenProfile(CitizenVM citizenVM)
        {
            try
            {

                if (citizenVM != null)
                {
                    var CitizenGet=await ctx.UserProfiles.Where(x=>x.ProfileId==citizenVM.ProfileId && x.IsDeleted==false).FirstOrDefaultAsync();

                    if (CitizenGet!=null)
                    {
                        CitizenGet.FullName = citizenVM.FullName;
                        CitizenGet.FatherName=citizenVM.FatherName;
                        CitizenGet.CNICNo = citizenVM.CNICNo;
                        CitizenGet.CityId = citizenVM.CityId;
                        //CitizenGet.CountryCode = citizenVM.CountryCode;
                        CitizenGet.ContactNumber=citizenVM.ContactNumber;
                        CitizenGet.CurrAddress=citizenVM.CurrAddress;
                        CitizenGet.PermAddress=citizenVM.PermAddress;

                        if (citizenVM.ProfilePhoto!=null)
                        {
                            UserDocument document = new UserDocument();
                            document.UserId = citizenVM.UserId;
                           document.DocName=citizenVM.ProfilePhoto.Name;
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
        public async Task<List<LawyerVM>> GetLawyerList(FilterVM filterVM)
        {
            List<LawyerVM> lawyerList = new List<LawyerVM>();
            //List<SqlParameter> sqlParameters= new List<SqlParameter>();
            try
            {

                if (filterVM != null)
                {
                    SqlParameter[] param=baseSP.CreateSqlParametersFromModel(filterVM);

                    var d = await baseSP.ExecuteStoredProcedureAsync<sp_GetCitizenLawyers_Result>("sp_GetCitizenLawyers", param);

                    if (d != null)
                    {
                        foreach (var item in d)
                        {
                            lawyerList.Add(new LawyerVM
                            {
                                LawyerId = item.UserId,
                                UserName = item.FullName,
                                TotalExperience = item.TotalExperience,
                                Rating = item.Rating,
                                IsFavourite = item.IsFavourite??false
                            });
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lawyerList;
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
                    lawyerHome.CompltedCase = await ctx.CourtCases.CountAsync(x => x.LawyerId== lawyerId && x.CaseStatusId==(int)CaseStatus.Initiated && x.IsDeleted==false);


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
                                                  CaseStatusId=c.CaseStatusId
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
                    && (x.RoleId == (int)Roles.Lawyer && x.IsActive == true && x.IsVerified == true && x.IsDeleted == false))
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
    }
}
