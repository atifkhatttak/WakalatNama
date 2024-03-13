﻿using Business.BusinessLogic;
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
    public interface ISettingsRepository
    {
        Task<List<Country>> GetAllCountries();
        Task<List<City>> GetAllCities(int countryId=0);
        Task<CasesDropDownVM> GetCaseDropDown();
        Task<List<LawyerFeeStructureVM>> GetLawyerFeeStructure();
        Task<List<LawyerFeeStructureVM>> GetLawyerFeeByCatAndJurisdiction(GetLawyerFeeVM feeVM);
        Task<LawyerFeeStructureVM> GetSingleFee(GetLawyerFeeVM feeVM);
    }
}
