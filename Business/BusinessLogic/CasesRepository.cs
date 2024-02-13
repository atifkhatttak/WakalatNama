using Business.Services;
using Business.ViewModels;
using Data.Context;
using Data.DomainModels;
using Microsoft.AspNetCore.Identity;
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
                        CreatedUser=caseVM.CitizenId,
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

        public async Task<List<CourtCaseVM>> GetCitizenCases(long? userId)
        {
            List<CourtCaseVM> userCases = null;
            try
            {
                if (userId != null && userId > 0)
                {
                    var caseList = ctx.Set<CourtCases>().Where(x => x.CitizenId == userId && x.IsDeleted==false).ToList();
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
    }
}
