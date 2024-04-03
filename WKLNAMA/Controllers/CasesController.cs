using Business.BusinessLogic;
using Business.Enums;
using Business.Helpers;
using Business.Services;
using Business.ViewModels;
using Data.DomainModels;
using Google.Apis.Drive.v3.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjWakalatnama.DataLayer.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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

        [SwaggerOperation(Summary ="Create/Update case from here")]
        [HttpPost("CreateUpdateCase")]
        public async Task<ActionResult> CreateUpdateCase([FromForm]CourtCaseVM courtCase)
        {
            return await APIResponse(async () => {
                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await casesRepository.CreateUpdateCase(courtCase);

                return Ok(apiResponse);
            });            
        }
        [SwaggerOperation(Summary = "Get list of citizen cases by citizenId")]
        [HttpGet("GetCitizenCases")]
       //[Authorize(Roles = "Citizen")]
        public async Task<ActionResult> GetCitizenCases([Required]long? userId)
        {
            return await APIResponse(async () => {
                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await casesRepository.GetCitizenCases(userId);

                return Ok(apiResponse);
            });           
        }
        [SwaggerOperation(Summary = "Get list of Lawyer cases by lawyerid")]
        [HttpGet("GetLawyerCases")]        
        //[Authorize(Roles = "Lawyer")]
        public async Task<ActionResult> GetLawyerCases([Required] long? userId)
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
        public async Task<ActionResult> GetCitizenDateList([Required]long? userId,[Required]long caseId)
        {
                return await APIResponse(async () => {

                    apiResponse.Message = HttpStatusCode.OK.ToString();
                    apiResponse.HttpStatusCode = HttpStatusCode.OK;
                    apiResponse.Success = true;
                    apiResponse.Data = await casesRepository.GetCitizenDateList(userId, caseId);

                    return Ok(apiResponse);
                });
        }
        [SwaggerOperation(Summary = "Get list of lawyer cases dates, by lawyerId")]
        [HttpGet("GetLawyerDateList")]
        public async Task<ActionResult> GetLawyerDateList([Required]long? userId,[Required]long caseId)
        {
            return await APIResponse(async () => {

                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await casesRepository.GetLawyerDateList(userId, caseId);

                return Ok(apiResponse);
            });
        }
        [SwaggerOperation(Summary ="Get single case for edit by caseid")]
        [HttpGet("GetCaseById")]        
        public async Task<ActionResult> GetCaseById([Required] long? caseId)
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

        [SwaggerOperation(Summary = "List of cases for Admin Approval")]
        [HttpGet("GetCasesForAdminApproval")]
        public async Task<ActionResult> GetCasesForAdminApproval()
        {
            try
            {
                List<CourtCaseVM> _caseList = new List<CourtCaseVM>();

                if (UserModel.Role==Roles.Admin.ToString())
                 _caseList = await casesRepository.GetCasesForAdminApproval();
                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = _caseList;
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

        [SwaggerOperation(Summary = "List of cases for Lawyer Approval")]
        [HttpGet("GetCasesForLawyerApproval")]
        public async Task<ActionResult> GetCasesForLawyerApproval()
        {
            try
            {
                List<CourtCaseVM> _caseList = new List<CourtCaseVM>();

                if (UserModel.Role == Roles.Lawyer.ToString())
                    _caseList = await casesRepository.GetCasesForLawyerApproval(UserModel.UserId);
                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = _caseList;
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
        [HttpPost("AcceptRejectCaseByLawyer")]
        public async Task<ActionResult> AcceptRejectCaseByLawyer(AcceptRejectCaseVM acceptReject)
        {
            try
            {
                acceptReject.DecisionUserId = UserModel.UserId;
                bool isValid = false;

                isValid =  await casesRepository.AcceptRejectCase(acceptReject);

                if (isValid &&  acceptReject.Status == (int)CaseStatuses.LawyerRejected)
                {
                    await casesRepository.AddCaseRejectionReason(acceptReject);
                }
                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = isValid;
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

        [SwaggerOperation(Summary = "Accept/Reject new case on Admin end")] 
        [HttpPost("AcceptRejectCaseByAdmin")]
        public async Task<ActionResult> AcceptRejectCaseByAdmin(AcceptRejectCaseVM acceptReject)
        {
            try 
            {
                acceptReject.DecisionUserId = UserModel.UserId;

               var isValid=  await casesRepository.AcceptRejectCase(acceptReject);

                if (isValid && acceptReject.Status==(int)CaseStatuses.AdminRejected)
                {
                  await  casesRepository.AddCaseRejectionReason(acceptReject);
                 }
                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = isValid;
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

        [SwaggerOperation(Summary = "Assign Employee To Case")]
        [HttpPost("AssignEmployeeToCase")]
        public async Task<ActionResult> AssignEmployeeToCase(CourtCaseVM model)  
        {
            try
            {
                bool isValid = false;
                if(UserModel.Role==Roles.Admin.ToString() || UserModel.Role==Roles.Zonal_Manager.ToString())
                  isValid = await casesRepository.AssignEmployeeToCase(model);

                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true; 
                apiResponse.Data = isValid;
            }
            catch (Exception ex)
            {
                apiResponse.Message = ex.Message;
                apiResponse.HttpStatusCode = HttpStatusCode.InternalServerError;
                apiResponse.Success = false;
                apiResponse.Data = false;
            }
            return Ok(apiResponse);
        }

        [SwaggerOperation(Summary = "List of Rejected Cases wit Reason")]
        [HttpGet("RejectedCaseswithReason")]
        public async Task<ActionResult> RejectedCaseswithReason()
        {
            try
            {
                List<CaseRejectionReasonVm> _caseRejectionReasons = new List<CaseRejectionReasonVm>() ; 
                if (UserModel.Role == Roles.Admin.ToString() || UserModel.Role == Roles.Zonal_Manager.ToString())
                    _caseRejectionReasons = await casesRepository.GetCaseRejectionReason();

                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = _caseRejectionReasons;
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
    }
}
