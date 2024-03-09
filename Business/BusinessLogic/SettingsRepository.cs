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
            return await ctx.Countries.Where(x => x.IsDeleted == false).ToListAsync();
        }

        public async Task<List<City>> GetAllCities(int countryId=0)
        {            
            return await ((countryId==0)?ctx.Cities.Where(x=>x.IsDeleted==false):ctx.Cities.Where(x=>x.CountryId==countryId && x.IsDeleted == false)).ToListAsync();
        }

        public async Task<CasesDropDownVM> GetCaseDropDown()
        {
            CasesDropDownVM dropDownVM = new CasesDropDownVM();
            try
            {

                dropDownVM.Cities = await ctx.Cities
                    .AsNoTracking()
                    .Where(x=>x.IsDeleted==false)
                    .Select(city => new CityVM{ ID=city.Id,CityName=city.CityName }).ToListAsync();

                dropDownVM.CaseJurisdictions =await ctx.CaseJurisdictions
                    .AsNoTracking()
                    .Where(x => x.IsDeleted == false)
                    .Select(c => new CaseJurisdictionVM{ ID=c.CaseJurisdictionId,CaseJurisdiction=c.JurisdictionName }).ToListAsync();

                dropDownVM.CaseNature =await ctx.CaseCategories
                    .AsNoTracking()
                    .Where(x => x.IsDeleted == false)
                    .Select(c => new CategoryVM{ ID=c.ID,CategoryName=c.CategoryName }).ToListAsync();

                dropDownVM.PartyStatuses =await ctx.PartyStatuses
                    .AsNoTracking()
                    .Where(x => x.IsDeleted == false)
                    .Select(c => new PartyStatusVM{ ID=c.ID,StatusName=c.StatusName }).ToListAsync();

                dropDownVM.CasePursueds=new List<CasePursuedVM>() { new CasePursuedVM() {ID=1,CasePursued="District Court" } };
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dropDownVM;
        }
    }
}
