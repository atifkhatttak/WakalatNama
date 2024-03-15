using Business.BusinessLogic;
using Business.Enums;
using Business.Helpers;
using Business.Services;
using Business.ViewModels;
using Data.DomainModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjWakalatnama.DataLayer.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using WKLNAMA.Models;

namespace WKLNAMA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class CasesController : BaseController<CasesDetail>
    {
        private readonly ICasesRepository casesRepository;

        private ApiResponse apiResponse = new ApiResponse();
        public CasesController(ICasesRepository casesRepository, IHttpContextAccessor httpContextAccessor, IDocumentService documentService) : base(casesRepository, httpContextAccessor)
        {
            this.casesRepository = casesRepository;
        }

        [SwaggerOperation(Summary ="Create new case from here")]
        [HttpPost("CreateCase")]
        public async Task<ActionResult> CreateCase([FromForm]CourtCaseVM courtCase)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    apiResponse.Message = HttpStatusCode.BadRequest.ToString();
                    apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                    apiResponse.Success = false;
                    apiResponse.Data = courtCase;
                }

               var result =await casesRepository.CreateCase(courtCase);
                apiResponse.Message = HttpStatusCode.Created.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.Created;
                apiResponse.Success = true;
                apiResponse.Data = result;
            }
            catch (Exception ex)
            {
                apiResponse.Message = ex.Message;
                apiResponse.HttpStatusCode = HttpStatusCode.InternalServerError;
                apiResponse.Success = false;
                apiResponse.Data = null;
            }

            //return Ok(Task.FromResult(_viewModel));
            return Ok(apiResponse);
        }
        [SwaggerOperation(Summary = "Get list of citizen cases by citizenId")]
        [HttpGet("GetCitizenCases")]
       //[Authorize(Roles = "Citizen")]
        public async Task<ActionResult> GetCitizenCases(long? userId)
        {
            try
            {
              var result=  await casesRepository.GetCitizenCases(userId);
                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = result;
            }
            catch (Exception ex)
            {
                apiResponse.Message = ex.Message;
                apiResponse.HttpStatusCode = HttpStatusCode.InternalServerError;
                apiResponse.Success = false;
                apiResponse.Data = null;
            }
            return Ok(apiResponse);
        }
        [SwaggerOperation(Summary = "Get list of Lawyer cases by lawyerid")]
        [HttpGet("GetLawyerCases")]        
        //[Authorize(Roles = "Lawyer")]
        public async Task<ActionResult> GetLawyerCases(long? userId)
        {
            try
            {
                var result = await casesRepository.GetLawyerCases(userId);
                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = result;
            }
            catch (Exception ex)
            {
                apiResponse.Message = ex.Message;
                apiResponse.HttpStatusCode = HttpStatusCode.InternalServerError;
                apiResponse.Success = false;
                apiResponse.Data = null;
            }
            return Ok(apiResponse);
        }
        [SwaggerOperation(Summary = "Get list of citizen cases dates, by citizenId")]
        [HttpGet("GetCitizenDateList")]
        public async Task<ActionResult> GetCitizenDateList(long? userId)
        {
            try
            {
                var result = await casesRepository.GetCitizenDateList(userId);
                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = result;
            }
            catch (Exception ex)
            {
                apiResponse.Message = ex.Message;
                apiResponse.HttpStatusCode = HttpStatusCode.InternalServerError;
                apiResponse.Success = false;
                apiResponse.Data = null;
            }
            return Ok(apiResponse);
        }
        [SwaggerOperation(Summary = "Get list of lawyer cases dates, by lawyerId")]
        [HttpGet("GetLawyerDateList")]
        public async Task<ActionResult> GetLawyerDateList(long? userId)
        {
            try
            {
                var result = await casesRepository.GetLawyerDateList(userId);
                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = result;
            }
            catch (Exception ex)
            {
                apiResponse.Message = ex.Message;
                apiResponse.HttpStatusCode = HttpStatusCode.InternalServerError;
                apiResponse.Success = false;
                apiResponse.Data = null;
            }
            return Ok(apiResponse);
        }
        [SwaggerOperation(Summary ="Get single case for edit by caseid")]
        [HttpGet("GetCaseById")]        
        public async Task<ActionResult> GetCaseById(long? caseId)
        {
            try
            {
                var result = await casesRepository.GetCaseById(caseId);
                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = result;
            }
            catch (Exception ex)
            {
                apiResponse.Message = ex.Message;
                apiResponse.HttpStatusCode = HttpStatusCode.InternalServerError;
                apiResponse.Success = false;
                apiResponse.Data = null;
            }
            return Ok(apiResponse);
        }
        [SwaggerOperation(Summary = "Accept/Reject new case on lawyer end")]
        [HttpGet("AcceptRejectCaseByLawyer")]
        public async Task<ActionResult> AcceptRejectCaseByLawyer(AcceptRejectCaseVM acceptReject)
        {
            try
            {
                await casesRepository.AcceptRejectCaseByLawyer(acceptReject);
                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = null;
            }
            catch (Exception ex)
            {
                apiResponse.Message = ex.Message;
                apiResponse.HttpStatusCode = HttpStatusCode.InternalServerError;
                apiResponse.Success = false;
                apiResponse.Data = null;
            }
            return Ok(apiResponse);
        }
        [SwaggerOperation(Summary = "Create/Update Case Date on lawyer end")]
        [HttpPost("CreateUpdateCaseDate")]
        public async Task<ActionResult> CreateUpdateCaseDate([FromForm] CaseDateVM vM)
        {
                return await APIResponse(async () => {
                   
                    apiResponse.Message = HttpStatusCode.OK.ToString();
                    apiResponse.HttpStatusCode = HttpStatusCode.OK;
                    apiResponse.Success = true;
                    apiResponse.Data = await casesRepository.CreateUpdateCaseDate(vM);

                    return Ok(apiResponse);
                });
        }
    }
}
