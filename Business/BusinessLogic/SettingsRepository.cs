using Business.Enums;
using Business.Services;
using Business.ViewModels;
using Data.Context;
using Data.DomainModels;
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
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Business.BusinessLogic
{
    public class SettingsRepository : ISettingsRepository
    {
        private readonly WKNNAMADBCtx ctx;
        private readonly IConfiguration config;
        //private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly bool isAuthenticated = false;
        private readonly long LoggedInUserId = -1;
        //private readonly string LoggedInRole = "";

        public SettingsRepository(WKNNAMADBCtx ctx, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            this.ctx = ctx;
            this.config = config;
            //this._httpContextAccessor = httpContextAccessor;

            this.isAuthenticated = httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
            this.LoggedInUserId = isAuthenticated ? Convert.ToInt64(httpContextAccessor.HttpContext.User.FindFirstValue("UserId")) : -1;
            //this.LoggedInUserId = isAuthenticated ? Convert.ToInt64(httpContextAccessor.HttpContext.User.FindFirstValue("role")) : -1;
        }

        public async Task<List<Country>> GetAllCountries()
        {
            return await ctx.Countries.Where(x => !x.IsDeleted).ToListAsync();
        }

        public async Task<List<City>> GetAllCities(int countryId = 0)
        {
            return await ((countryId == 0) ? ctx.Cities.Where(x => !x.IsDeleted) : ctx.Cities.Where(x => x.CountryId == countryId && !x.IsDeleted)).ToListAsync();
        }

        public async Task<CasesDropDownVM> GetCaseDropDown()
        {
            CasesDropDownVM dropDownVM = new CasesDropDownVM();
            try
            {

                dropDownVM.Cities = await ctx.Cities
                    .AsNoTracking()
                    .Where(x => !x.IsDeleted)
                    .Select(city => new CityVM { ID = city.Id, CityName = city.CityName }).ToListAsync();

                dropDownVM.CaseJurisdictions = await ctx.CaseJurisdictions
                    .AsNoTracking()
                    .Where(x => x.IsDeleted == false)
                    .Select(c => new CaseJurisdictionVM { ID = c.CaseJurisdictionId, CaseJurisdiction = c.JurisdictionName }).ToListAsync();

                dropDownVM.CaseNature = await ctx.CaseCategories
                    .AsNoTracking()
                    .Where(x => !x.IsDeleted)
                    .Select(c => new CategoryVM { ID = c.ID, CategoryName = c.CategoryName }).ToListAsync();

                dropDownVM.PartyStatuses = await ctx.PartyStatuses
                    .AsNoTracking()
                    .Where(x => !x.IsDeleted)
                    .Select(c => new PartyStatusVM { ID = c.ID, StatusName = c.StatusName }).ToListAsync();

                dropDownVM.CasePursueds = new List<CasePursuedVM>() { new CasePursuedVM() { ID = 1, CasePursued = "District Court" } };
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dropDownVM;
        }

        public async Task<List<LawyerFeeStructureVM>> GetLawyerFeeStructure()
        {
            return await ctx.LawyerFeeStructures.Where(x => !x.IsDeleted).Select(x => new LawyerFeeStructureVM()
            {
                FeeId = x.FeeId,
                LawyerFee = x.LawyerFee,
                ExpMin = x.ExpMin,
                ExpMax = x.ExpMax,
                CaseNatureId = x.CaseNatureId,
                JurisdictionId = x.JurisdictionId,
                IsForeignQualified = x.IsForeignQualified

            }).ToListAsync();
        }

        public async Task<LawyerFeeStructureVM> GetSingleFee(GetLawyerFeeVM vM)
        {
            return (await ctx.LawyerFeeStructures
                 .Where(x => x.CaseNatureId == vM.CaseNatureId && x.JurisdictionId == vM.JurisdictionId && !x.IsDeleted
                && x.ExpMin == vM.ExpMin && x.ExpMax == vM.ExpMax)
                 .Select(x => new LawyerFeeStructureVM()
                 {
                     FeeId = x.FeeId,
                     LawyerFee = x.LawyerFee,
                     ExpMin = x.ExpMin,
                     ExpMax = x.ExpMax,
                     CaseNatureId = x.CaseNatureId,
                     JurisdictionId = x.JurisdictionId,
                     IsForeignQualified = x.IsForeignQualified
                 })
                 .FirstOrDefaultAsync())!;
        }       
        public async Task<List<LawyerFeeStructureVM>> GetLawyerFeeByCatAndJurisdiction(GetLawyerFeeVM feeVM)
        {
            return await ctx.LawyerFeeStructures
                .Where(x => x.CaseNatureId == feeVM.CaseNatureId && x.JurisdictionId == feeVM.JurisdictionId && !x.IsDeleted)
                .Select(x => new LawyerFeeStructureVM()
                {
                    FeeId = x.FeeId,
                    LawyerFee = x.LawyerFee,
                    ExpMin = x.ExpMin,
                    ExpMax = x.ExpMax,
                    CaseNatureId = x.CaseNatureId,
                    JurisdictionId = x.JurisdictionId,
                    IsForeignQualified = x.IsForeignQualified
                })
                .ToListAsync();
        }

        #region Documents

        public async Task<List<DownloadableDocsVM>> GetDownloadableDocuments(int docType)
        {
            List<DownloadableDocsVM> docList = new List<DownloadableDocsVM>();
            try
            {
                EDocumentType type = (EDocumentType)docType;

                switch (type)
                {
                    case EDocumentType.CitizenDownloadable:
                        docList = await ctx.UserDocuments
                            .Where(x => x.DocTypeId == (int)EDocumentType.CitizenDownloadable && x.IsUploaded == true && !x.IsDeleted)
                            .Select(x => new DownloadableDocsVM()
                            {
                                DocId = x.DocumentId,
                                DocName = x.DocName,
                                DocSize = 1,
                                DocPath = x.DocPath,
                                DocForUserType = x.DocTypeId

                            })
                            .ToListAsync();
                        break;
                    case EDocumentType.LawyerDownloadable:
                        docList = await ctx.UserDocuments
                            .Where(x => x.DocTypeId == (int)EDocumentType.LawyerDownloadable && x.IsUploaded == true && !x.IsDeleted)
                            .Select(x => new DownloadableDocsVM()
                            {
                                DocId = x.DocumentId,
                                DocName = x.DocName,
                                DocSize = 1,
                                DocPath = x.DocPath,
                                DocForUserType = x.DocTypeId

                            })
                            .ToListAsync();
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return docList;
        }

        public async Task<bool> UpLoadUserDocuments(UserDocumentVM userDoc)
        {
            bool response = false;
            try
            {
                if (userDoc!=null)
                {
                   UserDocument user= new UserDocument()
                    {
                        UserId = userDoc.UserId,
                        DocName = userDoc.DocName,
                        DocPath = userDoc.DocPath,
                        DocExtension = userDoc.DocExtension,
                        DocTypeId = userDoc.DocTypeId,
                        IsUploaded = userDoc.IsUploaded ?? false,
                        IsDeleted = false
                    };
                   await ctx.AddAsync(user);

                    await ctx.SaveChangesAsync();

                    response = (user.DocumentId > 0);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }
        public async Task<bool> DeleteUserDocument(long docId,int docType)
        {
            bool response = false;
            try
            {
                if (docId>0)
                {
                    var CheckExist=await ctx.UserDocuments.FindAsync(docId);

                    if (CheckExist != null)
                    {
                        CheckExist.IsDeleted = true;
                        await ctx.SaveChangesAsync();
                        response = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }

        public async Task<List<CaseStatusesVM>> GetCaseStatusByCaseId(long CaseId)
        {
            List<CaseStatusesVM> caseStatuses = new List<CaseStatusesVM>();
            try
            {
                var GetCase = await ctx.CourtCases.Where(x => x.CaseId == CaseId && !x.IsDeleted).FirstOrDefaultAsync();

                if (GetCase != null)
                {
                    caseStatuses = await (from cs in ctx.CategoriesStatuses
                                   join s in ctx.CaseStatuses on cs.CaseStatusId equals s.StatusId
                                   where cs.CategoryId == GetCase.CategoryId && !cs.IsDeleted
                                   select new CaseStatusesVM
                                   {
                                       ID = s.StatusId,
                                       Status = s.Status
                                   }).ToListAsync();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return caseStatuses;
        }

        #endregion
    }
}
