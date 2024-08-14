using Business.BusinessLogic;
using Business.Helpers;
using Business.Services;
using Business.ViewModels;
using Data.DomainModels;
using Google.Apis.Drive.v3.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProjWakalatnama.DataLayer.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Runtime.CompilerServices;
using WKLNAMA.Models;

namespace WKLNAMA.Controllers
{
    [EnableCors("CORSPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : CustomBaseController
    {
        private readonly ISettingsRepository settingsRepository;
        //private readonly ILogger _logger;
        public SettingsController(ISettingsRepository settingsRepository, IHttpContextAccessor httpContextAccessor, IDocumentService documentService)
            :base(httpContextAccessor)
        {
            this.settingsRepository = settingsRepository;
            //this._logger = logger;
        }
        [HttpGet("GetAllCities")]
        public async Task<ActionResult> GetAllCities(int countryId)
        {
            return await APIResponse(async () => {

                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await settingsRepository.GetAllCities(countryId);

                return Ok(apiResponse);
            });
        }
        [HttpGet("GetAllCountries")]
        public async Task<ActionResult> GetAllCountries()
        {
            return await APIResponse(async () => {

                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await settingsRepository.GetAllCountries();

                return Ok(apiResponse);
            });
        }

        [HttpGet("GetCasesDropDown")]
        public async Task<ActionResult> GetCasesDropDown()
        {
            return await APIResponse(async () => {

                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await settingsRepository.GetCaseDropDown();

                return Ok(apiResponse);
            });
        }
        [HttpGet("GetLawyerFeeStructure")]
        public async Task<ActionResult> GetFeeStructure()
        {
            return await APIResponse(async () => {

                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await settingsRepository.GetLawyerFeeStructure();

                return Ok(apiResponse);
            });
        }
        [HttpPost("GetLawyerFeeByCatAndJurisdictionId")]
        public async Task<ActionResult> GetLawyerFeeByCatAndJurisdiction(GetLawyerFeeVM vM)
        {
            if (!ModelState.IsValid)
            {
                return await APIResponse(async () => {

                    apiResponse.Message = ModelState.ValidationState.GetMessage();
                    apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                    apiResponse.Success = false;
                    apiResponse.Data = null;

                    return BadRequest(apiResponse);
                });
            }

            return await APIResponse(async () => {

                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await settingsRepository.GetLawyerFeeByCatAndJurisdiction(vM);

                return Ok(apiResponse);
            });
        }
        [HttpPost("GetSingleFee")]
        public async Task<ActionResult> SingleFees(GetLawyerFeeVM vM)
        {
            return await APIResponse(async () => {

                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await settingsRepository.GetSingleFee(vM);

                return Ok(apiResponse);
            });
        }
        [SwaggerOperation(Summary = "Get Case statuses by case id")]
        [HttpGet("GetCaseStatusesByCaseId")]
        public async Task<ActionResult> GetCaseStatusesByCaseId([Required]long CaseId)
        {
            return await APIResponse(async () => {
                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await settingsRepository.GetCaseStatusByCaseId(CaseId);

                return Ok(apiResponse);
            });
        }
        [SwaggerOperation(Summary ="Get Citizen/Lawyer downloadable documents")]
        [HttpGet("GetDownloadableDocuments")]
        public async Task<ActionResult> GetDownloadableDocuments([Required] int docType)
        {
            return await APIResponse(async () => {
                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await settingsRepository.GetDownloadableDocuments(docType);

                return Ok(apiResponse);
            });
        }
        [SwaggerOperation(Summary = "Upload user documents here on admin side")]
        [HttpPost("UploadUserDocuments")]
        public async Task<ActionResult> UploadUserDocuments(UserDocumentVM vM)
        {
            return await APIResponse(async () => {
                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await settingsRepository.UpLoadUserDocuments(vM);

                return Ok(apiResponse);
            });
        }
        [SwaggerOperation(Summary = "Delete user documents here on admin side")]
        [HttpDelete("DeleteUserDocuments")]
        public async Task<ActionResult> DeleteUserDocuments([Required]long docId,int docType)
        {
            return await APIResponse(async () => {
                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await settingsRepository.DeleteUserDocument(docId,docType);

                return Ok(apiResponse);
            });
        }
        [SwaggerOperation(Summary = "Get rejection reason list")]
        [HttpGet("GetRejectionList")]
        public async Task<ActionResult> GetRejectionList()
        {
            return await APIResponse(async () => {
                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await settingsRepository.GetRejectionList();

                return Ok(apiResponse);
            });
        }

        [SwaggerOperation(Summary = "Show banner content on citizen home screen top-Admin")]
        [HttpPost("UploadBannerContent")]
        public async Task<ActionResult> UploadBannerContent(UserBannerVM vM)
        {
            return await APIResponse(async () => {
                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await settingsRepository.UploadBannerContent(vM);

                return Ok(apiResponse);
            });
        }
        [SwaggerOperation(Summary = "Show banner list by user role id")]
        [HttpGet("GetBannerList")]
        public async Task<ActionResult> GetBannerList([Required]int RoleId)
        {
            return await APIResponse(async () => {
                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await settingsRepository.GetBannerList(RoleId);

                return Ok(apiResponse);
            });
        }
        [SwaggerOperation(Summary = "Delete banner-admin")]
        [HttpDelete("DeleteBanner/{BannerId}")]
        public async Task<ActionResult> DeleteBanner([Required]long BannerId)
        {
            return await APIResponse(async () => {
                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await settingsRepository.DeleteBanner(BannerId);

                return Ok(apiResponse);
            });
        }
    }
}
