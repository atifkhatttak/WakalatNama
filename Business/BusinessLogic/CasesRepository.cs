using Business.Enums;
using Business.Services;
using Business.ViewModels;
using Data.Context;
using Data.DomainModels;
using Google.Apis.Drive.v3.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProjWakalatnama.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Business.BusinessLogic
{
    public class CasesRepository : BaseRepository<CasesDetail>, ICasesRepository
    {
        private readonly WKNNAMADBCtx ctx;
        private readonly IConfiguration config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly bool isAuthenticated = false;
        private readonly long LoggedInUserId = -1;
        private readonly string LoggedInRole = "";

        public CasesRepository(WKNNAMADBCtx ctx, IConfiguration config,IHttpContextAccessor httpContextAccessor) : base(ctx)
        {
            this.ctx = ctx;
            this.config = config;
            this._httpContextAccessor = httpContextAccessor;

            this.isAuthenticated= _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
            this.LoggedInUserId= isAuthenticated ? Convert.ToInt64(_httpContextAccessor.HttpContext.User.FindFirstValue("UserId")) : -1;
            this.LoggedInUserId= isAuthenticated ? Convert.ToInt64(_httpContextAccessor.HttpContext.User.FindFirstValue("role")) : -1;
        }

        public async Task CreateCase(CourtCaseVM caseVM)
        {
            CourtCases courtCases = new CourtCases();
            try
            {
                if (!isAuthenticated) return;

                if (caseVM != null)
                {

                    courtCases.CitizenId = caseVM.CitizenId;
                    courtCases.LawyerId = caseVM.LawyerId;
                    courtCases.RedundantLawyerId = caseVM.RedundantLawyerId;
                    courtCases.CaseNumber = caseVM.CaseNumber;
                    courtCases.CaseTitle = caseVM.CaseTitle;
                    courtCases.PartyId = caseVM.PartyId;
                    courtCases.CategoryId = caseVM.CategoryId;
                    courtCases.CaseDescription = caseVM.CaseDescription!;
                    courtCases.CaseJurisdictionId = caseVM.CaseJurisdictionId;
                    courtCases.CourtId = caseVM.CourtId;

                   await ctx.AddAsync(courtCases);
                   await ctx.SaveChangesAsync();
                }
            }
           catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CourtCaseVM> GetCaseById(long? caseId)
        {
            CourtCaseVM? courtCase = new CourtCaseVM();
            try
            {
                if (!isAuthenticated) return courtCase;

                if (caseId!=null)
                {
                    courtCase = await (from c in ctx.CourtCases
                                                   join u in ctx.UserProfiles on c.CitizenId equals u.UserId
                                                   join cat in ctx.CaseCategories on c.CategoryId equals cat.ID
                                                   where c.CaseId ==caseId && !u.IsDeleted && !c.IsDeleted
                                                   select new CourtCaseVM
                                                   {
                                                       UserFullName = u.FullName,
                                                       CaseId = c.CaseId,
                                                       CitizenId = c.CitizenId,
                                                       LawyerId = c.LawyerId,
                                                       CaseNumber = c.CaseNumber,
                                                       CaseTitle = c.CaseTitle,
                                                       CaseDescription=c.CaseDescription,
                                                       CategoryId = c.CategoryId,
                                                       CategoryName = cat.CategoryName,
                                                       CaseStatusId = c.CaseStatusId
                                                   })
                          .FirstOrDefaultAsync();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return courtCase;
        }
        public async Task<List<CourtCaseVM>> GetCitizenCases(long? userId)
        {
            List<CourtCaseVM> userCases =new List<CourtCaseVM>();
            try
            {
                //if (!isAuthenticated) return userCases;
                //if (isAuthenticated)
                //{
                //    if (LoggedInUserId != userId) 
                //        return userCases;
                //}
                

                if (userId != null && userId > 0)
                {
                        var caseList = await (from u in ctx.UserProfiles
                                              join c in ctx.CourtCases on u.UserId equals c.CitizenId
                                              join cat in ctx.CaseCategories on c.CategoryId equals cat.ID
                                              where c.CitizenId == userId && u.RoleId == (int)Roles.Citizen && (u.IsDeleted == false && c.IsDeleted == false)
                                              select new CourtCaseVM
                                              {
                                                  UserId = userId,
                                                  UserFullName = u.FullName,
                                                  CaseId = c.CaseId,
                                                  CitizenId = c.CitizenId,
                                                  LawyerId = c.LawyerId,
                                                  CaseNumber = c.CaseNumber,
                                                  CaseTitle = c.CaseTitle,
                                                  CategoryId = c.CategoryId,
                                                  CategoryName = cat.CategoryName
                                              })
                          .ToListAsync();

                        if (caseList.Any())
                        {
                            foreach (var item in caseList)
                            {
                                userCases.Add(new CourtCaseVM
                                {
                                    UserId = userId,
                                    UserFullName = item.UserFullName,

                                    CaseId = item.CaseId,
                                    CitizenId = item.CitizenId,
                                    LawyerId = item.LawyerId,
                                    CaseNumber = item.CaseNumber,
                                    CaseTitle = item.CaseTitle,
                                    CategoryId = item.CategoryId,
                                    CategoryName = item.CategoryName
                                });
                            }
                        }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return userCases;
        }

        public async Task<List<CourtCaseVM>> GetLawyerCases(long? userId)
        {
            List<CourtCaseVM> userCases = new List<CourtCaseVM>();
            try
            {
                //if (!isAuthenticated) return userCases;
                //if (isAuthenticated)
                //{
                //    if (LoggedInUserId != userId) 
                //        return userCases;
                //}

                if (userId != null && userId > 0)
                {
                    var caseList = await (from u in ctx.UserProfiles
                                          join c in ctx.CourtCases on u.UserId equals c.LawyerId
                                          join cat in ctx.CaseCategories on c.CategoryId equals cat.ID
                                          where u.UserId == userId && u.RoleId == (int)Roles.Lawyer && (u.IsDeleted == false && c.IsDeleted == false)
                                          select new CourtCaseVM
                                          {
                                              UserId = userId,
                                              UserFullName = u.FullName,
                                              CaseId = c.CaseId,
                                              CitizenId = c.CitizenId,
                                              LawyerId = c.LawyerId,
                                              CaseNumber = c.CaseNumber,
                                              CaseTitle = c.CaseTitle,
                                              CategoryId = c.CategoryId,
                                              CategoryName = cat.CategoryName
                                          })
                      .ToListAsync();

                    if (caseList.Any())
                    {
                        foreach (var item in caseList)
                        {
                            userCases.Add(new CourtCaseVM
                            {
                                UserId = userId,
                                UserFullName = item.UserFullName,

                                CaseId = item.CaseId,
                                CitizenId = item.CitizenId,
                                LawyerId = item.LawyerId,
                                CaseNumber = item.CaseNumber,
                                CaseTitle = item.CaseTitle,
                                CategoryId = item.CategoryId,
                                CategoryName = item.CategoryName
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return userCases;
        }

        public async Task<List<CaseDetailVM>> GetCitizenDateList(long? userId)
        {
            
            List<CaseDetailVM> caseDetails = new List<CaseDetailVM>();
            try
            {
                //if (!isAuthenticated) return caseDetails;
                //if (isAuthenticated)
                //{
                //    if (LoggedInUserId != userId) 
                //        return caseDetails;
                //}

                if (userId != null && userId > 0)
                {
                    var caseList =
                       await (from cc in ctx.CourtCases
                         join cd in ctx.CasesDetails on cc.CaseId equals cd.CaseId
                         join cdd in ctx.CasesDocuments on cc.CaseId equals cdd.CaseId
                         where cc.CitizenId == userId && cc.IsDeleted==false
                         select new
                         {
                             cc.CaseId,
                             cc.CitizenId,
                             cc.LawyerId,
                             cc.CaseNumber,
                             cc.CaseTitle,
                             cd.CaseDateTitle,
                             cd.HearingDate,
                             cd.DateDescription,
                             cd.CaseStatusId,
                             cdd.DocName
                         }).ToListAsync();

                    if (caseList.Any())
                    {
                        foreach (var item in caseList)
                        {
                            caseDetails.Add(new CaseDetailVM
                            {
                                CaseId = item.CaseId,
                                CitizenId = item.CitizenId,
                                LawyerId = item.LawyerId,
                                CaseNumber = item.CaseNumber,
                                CaseTitle = item.CaseTitle,
                                DateTitle = item.CaseDateTitle,
                                HearingDate = item.HearingDate,
                                DateDescription = item.DateDescription,
                                CaseStatusId = item.CaseStatusId,
                                DocName= item.DocName
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return caseDetails;
        }
        public async Task<List<CaseDetailVM>> GetLawyerDateList(long? userId)
        {

            List<CaseDetailVM> caseDetails = new List<CaseDetailVM>();
            try
            {
                //if (!isAuthenticated) return caseDetails;
                //if (isAuthenticated)
                //{
                //    if (LoggedInUserId != userId) 
                //        return caseDetails;
                //}

                if (userId != null && userId > 0)
                {
                    var caseList =
                       await (from cc in ctx.CourtCases
                              join cd in ctx.CasesDetails on cc.CaseId equals cd.CaseId
                              join cdd in ctx.CasesDocuments on cc.CaseId equals cdd.CaseId
                              where cc.LawyerId == userId && cc.IsDeleted == false
                              select new
                              {
                                  cc.CaseId,
                                  cc.CitizenId,
                                  cc.LawyerId,
                                  cc.CaseNumber,
                                  cc.CaseTitle,
                                  cd.CaseDateTitle,
                                  cd.HearingDate,
                                  cd.DateDescription,
                                  cd.CaseStatusId,
                                  cdd.DocName
                              }).ToListAsync();

                    if (caseList.Any())
                    {
                        foreach (var item in caseList)
                        {
                            caseDetails.Add(new CaseDetailVM
                            {
                                CaseId = item.CaseId,
                                CitizenId = item.CitizenId,
                                LawyerId = item.LawyerId,
                                CaseNumber = item.CaseNumber,
                                CaseTitle = item.CaseTitle,
                                DateTitle = item.CaseDateTitle,
                                HearingDate = item.HearingDate,
                                DateDescription = item.DateDescription,
                                CaseStatusId = item.CaseStatusId,
                                DocName = item.DocName
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return caseDetails;
        }

        public async Task AcceptRejectCaseByLawyer(AcceptRejectCaseVM acceptReject)
        {
            try
            {
                if (!isAuthenticated) return;

                if (acceptReject!=null)
                {
                    var casse = await ctx.CourtCases.FindAsync(acceptReject.CaseId);
                    if (casse!=null)
                    {
                        casse.CaseStatusId =(acceptReject.Status==0)? (int)CaseStatus.Initiated: (int)CaseStatus.Draft;                       
                        await ctx.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
