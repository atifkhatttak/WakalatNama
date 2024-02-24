using Business.Enums;
using Business.Services;
using Business.ViewModels;
using Data.Context;
using Data.DomainModels;
using Google.Apis.Drive.v3.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProjWakalatnama.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Business.BusinessLogic
{
    public class CasesRepository : BaseRepository<CasesDetail>, ICasesRepository
    {
        private readonly WKNNAMADBCtx ctx;
        private readonly IConfiguration config;

        public CasesRepository(WKNNAMADBCtx ctx, IConfiguration config) : base(ctx)
        {
            this.ctx = ctx;
            this.config = config;
        }

        public async Task CreateCase(CourtCaseVM caseVM)
        {
            CourtCases courtCases = null;
            try
            {
                if (caseVM != null)
                {
                    courtCases  = new CourtCases()
                    {
                        CitizenId = caseVM.CitizenId,
                        LawyerId= caseVM.LawyerId,
                        RedundantLawyerId=caseVM.RedundantLawyerId,
                        CaseNumber=caseVM.CaseNumber,
                        PartyId=caseVM.PartyId,
                        CategoryId=caseVM.CategoryId,
                        CaseDescription=caseVM.CaseDescription!,
                        CaseJurisdictionId=caseVM.CaseJurisdictionId,
                        CourtId=caseVM.CourtId,
                        CreatedDate=DateTime.Now,
                        CreatedBy=caseVM.CitizenId,
                        IsDeleted=false                    
                    };

                   await ctx.AddAsync(courtCases);
                   await ctx.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            //return courtCases;
        }

        public async Task<CourtCaseVM> GetCaseById(long? caseId)
        {
            CourtCaseVM? courtCase = new CourtCaseVM();
            try
            {
                if (caseId!=null)
                {
                    courtCase = await (from c in ctx.CourtCases
                                                   join u in ctx.UserProfiles on c.CitizenId equals u.UserId
                                                   join cat in ctx.CaseCategories on c.CategoryId equals cat.ID
                                                   where c.CaseId ==caseId && (u.IsDeleted == false && c.IsDeleted == false)
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

        public async Task<List<CourtCaseVM>> GetUserCases(long? userId)
        {
            List<CourtCaseVM> userCases = null;
            try
            {
                if (userId != null && userId > 0)
                {
                    var currUser=await ctx.UserProfiles.Where(x=>x.UserId==userId).FirstOrDefaultAsync();

                    if (currUser != null) {
                       
                       var caseList =await (from u in ctx.UserProfiles
                         join c in ctx.CourtCases on u.UserId equals c.CitizenId
                         join cat in ctx.CaseCategories on c.CategoryId equals cat.ID
                         where u.UserId==userId && u.RoleId==currUser.RoleId && (u.IsDeleted==false && c.IsDeleted == false)
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
                            userCases = new List<CourtCaseVM>();
                            foreach (var item in caseList)
                            {
                                userCases.Add(new CourtCaseVM
                                {
                                    UserId=userId,
                                    UserFullName = currUser.FullName,

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
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return userCases ?? new List<CourtCaseVM>();
        }
        public async Task<List<CaseDetailVM>> GetUserDateList(long? userId)
        {
            List<CaseDetailVM> caseDetails = null;
            try
            {
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
                        caseDetails = new List<CaseDetailVM>();
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
            return caseDetails ?? new List<CaseDetailVM>();
        }
        public async Task AcceptRejectCaseByLawyer(AcceptRejectCaseVM acceptReject)
        {
            try
            {
                if (acceptReject!=null)
                {
                    var casse = await ctx.CourtCases.FindAsync(acceptReject.CaseId);
                    if (casse!=null)
                    {
                        casse.ModifiedDate = DateTime.Now;
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
