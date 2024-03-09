using Business.BusinessLogic;
using Business.Services;
using Business.ViewModels;
using Data.DomainModels;
using Google.Apis.Drive.v3.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProjWakalatnama.DataLayer.Models;
using System.ComponentModel.DataAnnotations;
using System.Net;
using WKLNAMA.Models;

namespace WKLNAMA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingsRepository settingsRepository;

        private ApiResponse apiResponse = new ApiResponse();
        public SettingsController(ISettingsRepository settingsRepository, IHttpContextAccessor httpContextAccessor, IDocumentService documentService)
        {
            this.settingsRepository = settingsRepository;
        }
        [HttpGet("GetAllCities")]
        public async Task<ActionResult> GetAllCities(int countryId)
        {
            return Ok(new ApiResponse()
            {
                Message = HttpStatusCode.OK.ToString(),
                HttpStatusCode = HttpStatusCode.OK,
                Success = true,
                Data = await settingsRepository.GetAllCities(countryId)
            });
        }
        [HttpGet("GetAllCountries")]
        public async Task<ActionResult> GetAllCountries()
        {
            return Ok(new ApiResponse()
            {
                Message = HttpStatusCode.OK.ToString(),
                HttpStatusCode = HttpStatusCode.OK,
                Success = true,
                Data = await settingsRepository.GetAllCountries()
            });
        }

        [HttpGet("GetCasesDropDown")]
        public async Task<ActionResult> GetCasesDropDown()
        {
            return Ok(new ApiResponse()
            {
                Message = HttpStatusCode.OK.ToString(),
                HttpStatusCode = HttpStatusCode.OK,
                Success = true,
                Data = await settingsRepository.GetCaseDropDown()
            });
        }

    }
}
