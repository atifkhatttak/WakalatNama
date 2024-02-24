using Business.Services;
using Business.ViewModels;
using Data.Context;
using Data.DomainModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProjWakalatnama.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<List<CourtCaseVM>> GetUserCases(long? userId)
        {
            List<CourtCaseVM> userCases = null;
            try
            {
                if (userId != null && userId > 0)
                {
                    var caseList =await ctx.Set<CourtCases>().Where(x => x.CitizenId == userId && x.IsDeleted==false).ToListAsync();
                    if (caseList.Any())
                    {
                        userCases = new List<CourtCaseVM>();
                        foreach (var item in caseList)
                        {
                            userCases.Add(new CourtCaseVM
                            {
                                CaseId = item.CaseId,
                                CitizenId = item.CitizenId,
                                LawyerId = item.LawyerId,
                                CaseNumber = item.CaseNumber,
                                CaseTitle = item.CaseTitle,
                                CategoryId = item.CategoryId
                            });
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
    }
}
