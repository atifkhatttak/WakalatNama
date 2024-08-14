using Business.BusinessLogic;
using Business.Chat_Hub;
using Business.Enums;
using Business.Helpers;
using Business.Helpers.Extension;
using Business.Services;
using Business.ViewModels;
using Data.DomainModels;
using Google.Apis.Drive.v3.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjWakalatnama.DataLayer.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Runtime.Intrinsics.X86;
 using WKLNAMA.Models;

namespace WKLNAMA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    [AllowAnonymous]
    public class CasesController : BaseController<CasesDetail>
    {
        private readonly ICasesRepository casesRepository;
        private readonly IMessageRepository messageRepository;
        private readonly ChatHub chatHub;

        private ApiResponse apiResponse = new ApiResponse();
        public CasesController(ICasesRepository casesRepository, IHttpContextAccessor httpContextAccessor, IDocumentService documentService,IMessageRepository messageRepository,ChatHub chatHub) : base(casesRepository, httpContextAccessor)
        {
            this.casesRepository = casesRepository;
            this.messageRepository = messageRepository;
            this.chatHub = chatHub;
        }

        [SwaggerOperation(Summary ="Create/Update case from here-citizen")]
        [HttpPost("CreateUpdateCase")]
        public async Task<ActionResult> CreateUpdateCase([FromBody]CourtCaseVM courtCase)
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
        [SwaggerOperation(Summary = "Get list of citizen cases dates, by citizenId-Secured")]
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
        [SwaggerOperation(Summary = "Get list of lawyer cases dates, by lawyerId-Secured")]
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
            return await APIResponse(async () => {

                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await casesRepository.GetCaseById(caseId);

                return Ok(apiResponse);
            });          
        }
        [SwaggerOperation(Summary = "Get case detials with documents")]
        [HttpGet("GetCaseDetailsById")]
        public async Task<ActionResult> GetCaseDetailsById([Required] long? caseId)
        {
            return await APIResponse(async () => {

                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await casesRepository.GetCaseDetailsById(caseId);

                return Ok(apiResponse);
            });
        }
        [SwaggerOperation(Summary = "List of cases for Lawyer Approval")]
        [HttpGet("GetCasesForLawyerApproval")]
        public async Task<ActionResult> GetCasesForLawyerApproval()
        {
            try
            {
                List<CourtCaseVM> _caseList = new List<CourtCaseVM>();

                //if (UserModel.Role == Roles.Lawyer.ToString())
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

        [SwaggerOperation(Summary = "Accept/Reject new case on lawyer end-Secured")]
        [HttpPost("AcceptRejectCaseByLawyer")]
        public async Task<ActionResult> AcceptRejectCaseByLawyer(AcceptRejectCaseVM acceptReject)
        {
            try
            {
                acceptReject.DecisionUserId = UserModel.UserId;
                bool isValid = false;

                isValid =  await casesRepository.AcceptRejectCase(acceptReject);

                if (isValid &&  acceptReject.Status == (int)ECaseStatuses.LawyerRejected)
                {
                    await casesRepository.AddCaseRejectionReason(acceptReject);
                }


                //Send greeting message from lawyer to the citizen after case approval
                //#region send message to citizen on case approval   
                //try
                //{
                //    if (isValid && acceptReject.Status == (int)ECaseStatuses.Approved)
                //    {
                //        var caseData = casesRepository.GetCaseById(acceptReject.CaseId).Result;
                //        var createdMsg = messageRepository.Create(new MessageVm()
                //        {
                //            Content = "Hi",
                //            FromUserId = caseData.LawyerId,
                //            ToUserId = caseData.CitizenId,
                //            IsRead = false,
                //            ParentId = -1
                //        }).Result;
                //        chatHub.DirectMessage(createdMsg);
                //    }
                //}
                //catch (Exception ex)
                //{
                //}
                //#endregion


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
        [SwaggerOperation(Summary = "Create Case Date on lawyer end")]
        [HttpPost("CreateCaseDate")]
        public async Task<ActionResult> CreateCaseDate([FromBody]CaseDateVM vM)
        {
                return await APIResponse(async () => {
                   
                    apiResponse.Message = HttpStatusCode.OK.ToString();
                    apiResponse.HttpStatusCode = HttpStatusCode.OK;
                    apiResponse.Success = true;
                    apiResponse.Data = await casesRepository.CreateCaseDate(vM);

                    return Ok(apiResponse);
                });
        }
        [SwaggerOperation(Summary = "Update Case Date on lawyer end")]
        [HttpPost("UpdateCaseDate")]
        public async Task<ActionResult> UpdateCaseDate(CaseDateVM vM)
        {
            return await APIResponse(async () => {

                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await casesRepository.UpdateCaseDate(vM);

                return Ok(apiResponse);
            });
        }
        [SwaggerOperation(Summary = "Create/Update Case Date on lawyer end")]
        [HttpGet("GetCaseDate")]
        public async Task<ActionResult> GetCaseDate([Required]long CaseId, [Required] long DateId)
        {
            return await APIResponse(async () => {

                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await casesRepository.GetCaseDate(CaseId,DateId);

                return Ok(apiResponse);
            });
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



        //code here for admin api's
        #region Admin Apis


        [SwaggerOperation(Summary = "List of cases for Admin Approval")]
        [HttpGet("GetCasesForAdminApproval")]
        public async Task<ActionResult> GetCasesForAdminApproval()
        {
            try
            {
                List<CourtCaseVM> _caseList = new List<CourtCaseVM>();

                //if (UserModel.Role==Roles.Admin.ToString())
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
        [SwaggerOperation(Summary = "Accept/Reject new case on Admin end-Secured")]
        [HttpPost("AcceptRejectCaseByAdmin")]
        public async Task<ActionResult> AcceptRejectCaseByAdmin(AcceptRejectCaseVM acceptReject)
        {
            try
            {
                acceptReject.DecisionUserId = UserModel.UserId;

                var isValid = await casesRepository.AcceptRejectCase(acceptReject);

                if (isValid && acceptReject.Status == (int)ECaseStatuses.AdminRejected)
                {
                    await casesRepository.AddCaseRejectionReason(acceptReject);
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
        [SwaggerOperation(Summary = "GetAdminCaseDetails for new case on Admin end-Admin")]
        [HttpGet("GetAdminCaseDetails")]
        public async Task<ActionResult> GetAdminCaseDetails([Required]long CaseId)
        {
            return await APIResponse(async () => {

                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await casesRepository.GetAdminCaseDetails(CaseId);

                return Ok(apiResponse);
            });
        }
        [SwaggerOperation(Summary = "Update Case Status By Admin on Admin end")]
        [HttpPost("UpdateCaseStatusByAdmin")]
        public async Task<ActionResult> UpdateCaseStatusByAdmin([FromBody]AdminAcceptRejectCaseVM vM)
        {
            return await APIResponse(async () => {

                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await casesRepository.UpdateCaseStatusByAdmin(vM);

                return Ok(apiResponse);
            });
        }
        [SwaggerOperation(Summary = "Get Cases Report")]
        [HttpPost("GetCasesReport")]
        public async Task<ActionResult> GetCasesReport([FromBody]CasesCityWiseReportVm model)
        {
            return await APIResponse(async () => {

                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await casesRepository.GetCityCategoryLegalStatusReport(model);

                return Ok(apiResponse);
            });
        }
        [SwaggerOperation(Summary = "Get Cases Complete/Pending Count")]
        [HttpGet("CompletedPendingCasesCount")]
        public async Task<ActionResult> CompletedPendingCasesCount()
        {
            return await APIResponse(async () => {

                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await casesRepository.GetCompletedPendingCasesCount();

                return Ok(apiResponse);
            });
        }
        [SwaggerOperation(Summary = "Get Cases FinancialReport")]
        [HttpPost("GetCaseFinancialTransactonReport")]
        public async Task<ActionResult> GetCaseFinancialTransactonReport(FinancialReportFilterVm filters) 
        {
            return await APIResponse(async () => {

                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK; 
                apiResponse.Success = true;
                apiResponse.Data = await casesRepository.GetFinancialTransactionReport(filters);

                return Ok(apiResponse);
            });
        }
        [SwaggerOperation(Summary = "Create Case Date end-Admin")]
        [HttpPost("CreateCaseDateByAdmin")]
        public async Task<ActionResult> CreateCaseDateByAdmin([FromBody] NewCaseDateVM vM)
        {
            return await APIResponse(async () => {

                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await casesRepository.CreateCaseDateByAdmin(vM);

                return Ok(apiResponse);
            });
        }
        [SwaggerOperation(Summary = "Update Case Date-Admin")]
        [HttpPost("UpdateCaseDateByAdmin")]
        public async Task<ActionResult> UpdateCaseDateByAdmin([FromBody] NewCaseDateVM vM)
        {
            return await APIResponse(async () => {

                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await casesRepository.UpdateCaseDateByAdmin(vM);

                return Ok(apiResponse);
            });
        }
        [SwaggerOperation(Summary = "Create/Update Case Date-Admin")]
        [HttpGet("GetCaseDateByAdmin")]
        public async Task<ActionResult> GetCaseDateByAdmin([Required] long CaseId, [Required] long DateId)
        {
            return await APIResponse(async () => {

                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await casesRepository.GetCaseDateByAdmin(CaseId, DateId);

                return Ok(apiResponse);
            });
        }
        [SwaggerOperation(Summary = "Get All Approved and ongoing cases-Admin")]
        [HttpGet("GetAllCasesByAdmin")]
        public async Task<ActionResult> GetAllCasesByAdmin()
        {
            return await APIResponse(async () => {

                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await casesRepository.GetAllCasesByAdmin();

                return Ok(apiResponse);
            });
        }
        [SwaggerOperation(Summary = "Get list of All case dates, by case id-Secured-Admin")]
        [HttpGet("GetDateListByCaseId")]
        public async Task<ActionResult> GetDateListByCaseId([Required]long? userId, [Required] long caseId)
        {
            return await APIResponse(async () => {

                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await casesRepository.GetDateListByCaseId(userId, caseId);

                return Ok(apiResponse);
            });
        }
        [SwaggerOperation(Summary = "delete case date by admin,Secured-Admin")]
        [HttpPost("DeleteCaseDateByAdmin")]
        public async Task<ActionResult> DeleteCaseDateByAdmin([Required] long userId,[Required] long caseId, [Required] long dateId)
        {
            return await APIResponse(async () => {

                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await casesRepository.DeleteCaseDateByAdmin(userId, caseId,dateId);

                return Ok(apiResponse);
            });
        }
        [SwaggerOperation(Summary = "delete case, by admin-Secured-Admin")]
        [HttpPost("DeleteCaseByAdmin")]
        public async Task<ActionResult> DeleteCaseByAdmin([Required] long userId, [Required] long caseId)
        {
            return await APIResponse(async () => {

                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await casesRepository.DeleteCaseByAdmin(userId, caseId);

                return Ok(apiResponse);
            });
        }

        [SwaggerOperation(Summary = "GetApprovedCaseDetailsByAdmin for approved case on Admin end-Admin")]
        [HttpGet("GetApprovedCaseDetailsByAdmin")]
        public async Task<ActionResult> GetApprovedCaseDetailsByAdmin([Required] long CaseId)
        {
            return await APIResponse(async () => {

                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await casesRepository.GetApprovedCaseDetailsByAdmin(CaseId);

                return Ok(apiResponse);
            });
        }
        [SwaggerOperation(Summary = "Create/Update case by admin from here-admin")]
        [HttpPost("CreateUpdateCaseByAdmin")]
        public async Task<ActionResult> CreateUpdateCaseByAdmin([FromBody]AdminCourtCaseVM courtCase)
        {
            return await APIResponse(async () => {
                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await casesRepository.CreateUpdateCaseByAdmin(courtCase);

                return Ok(apiResponse);
            });
        }
        #endregion
    }
}
