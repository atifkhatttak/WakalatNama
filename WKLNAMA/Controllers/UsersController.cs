using Business.BusinessLogic;
using Business.Services;
using Business.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjWakalatnama.DataLayer.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Net;
using WKLNAMA.Models;

namespace WKLNAMA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseController<UserProfile>
    {
        private readonly IUserRepository userRepository;
        private ApiResponse apiResponse = new ApiResponse();

        public UsersController(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor) : base(userRepository, httpContextAccessor)
        {
            this.userRepository = userRepository;

        }
        [SwaggerOperation(Summary = "Create/update citizen profile from here- for now this api will only update")]
        [HttpPost("CreateUpdateCitizenProfile")]
        public async Task<ActionResult> CreateUpdateCitizenProfile([FromBody]CitizenVM citizenVM)
        { 
                return await APIResponse(async () => {
                    apiResponse.Message = HttpStatusCode.OK.ToString();
                    apiResponse.HttpStatusCode = HttpStatusCode.OK;
                    apiResponse.Success = true;
                    apiResponse.Data = await userRepository.CreateCitizenProfile(citizenVM);

                    return Ok(apiResponse);
                });
          
        }
        [SwaggerOperation(Summary = "Get citizen Home screen lawyer list from here")]
        [HttpPost("GetCitizenHome")]
        public async Task<ActionResult> GetCitizenHome(FilterVM filterVM)
        {
            return await APIResponse(async () => {
                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await userRepository.GetCitizenHome(filterVM);

                return Ok(apiResponse);
            });            
        }
        [SwaggerOperation(Summary = "Get lawyer profile details  from here")]
        [HttpGet("GetLawyerDetails")]
        public async Task<ActionResult> GetLawyerDetails(int? lawyerId)
        {
            return await APIResponse(async () => {
                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await userRepository.GetLawyerProfile(lawyerId);

                return Ok(apiResponse);
            });            
        }
        [SwaggerOperation(Summary = "Get lawyer home screen data from here")]
        [HttpGet("GetLayerHome")]
        public async Task<ActionResult> GetLayerHome(int? LawyerId)
        {
            return await APIResponse(async () => {
                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await userRepository.GetLawyerHome(LawyerId);

                return Ok(apiResponse);
            });            
        }
        [SwaggerOperation(Summary = "Get citizen profile details from here-secured")]
        [HttpGet("GetCitizenDetails")]
        public async Task<ActionResult> GetCitizenDetails(int? CitizenId)
        {
            return await APIResponse(async () => {
                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await userRepository.GetCitizenProfile(CitizenId);

                return Ok(apiResponse);
            });           
        }
        [SwaggerOperation(Summary = "Create/update lawyer profile from here")]
        [HttpPost("CreateUpdateLawyerProfile")]
        public async Task<ActionResult> CreateUpdateLawyerProfile([FromBody]LawyerProfileVM lawyerVM)
        {
            return await APIResponse(async () => {
                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await userRepository.CreateUpdateLawyerProfile(lawyerVM);

                return Ok(apiResponse);
            });

        }
        [SwaggerOperation(Summary = "Create/update lawyer Experties from here")]
        [HttpPost("CreateUpdateLawyerExperties")]
        public async Task<ActionResult> CreateUpdateLawyerExperties(List<LawyerExpertiesVM> expertiesVMs)
        {
            return await APIResponse(async () => {
                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await userRepository.CreateUpdateLawyerExperties(expertiesVMs);

                return Ok(apiResponse);
            });

        }
        [SwaggerOperation(Summary = "Create/update lawyer Qaulification from here")]
        [HttpPost("CreateUpdateLawyerQaulification")]
        public async Task<ActionResult> CreateUpdateLawyerQaulification(List<LawyerQualificationVM> qualificationVMs)
        {
            return await APIResponse(async () => {
                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await userRepository.CreateUpdateLawyerQaulification(qualificationVMs);

                return Ok(apiResponse);
            });

        }
        [SwaggerOperation(Summary = "Get lawyer profile information  from here-secured")]
        [HttpGet("GetLawyerProfileInfo")]
        public async Task<ActionResult> GetLawyerProfileInfo(int? lawyerId)
        {
            return await APIResponse(async () => {
                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await userRepository.GetLawyerProfileInfo(lawyerId);

                return Ok(apiResponse);
            });
        }
        [SwaggerOperation(Summary = "Get lawyer profile information  from here")]
        [HttpGet("GetLawyerProfileInfoForAdmin")]
        public async Task<ActionResult> GetLawyerProfileInfoForAdmin(int? lawyerId)
        {
            return await APIResponse(async () => {
                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await userRepository.GetLawyerProfileInfoForAdmin(lawyerId);

                return Ok(apiResponse);
            });
        }
        [SwaggerOperation(Summary = "Accept/Reject Lawyer By Admin  from here")]
        [HttpPost("AcceptRejectLawyerByAdmin")]
        public async Task<ActionResult> AcceptRejectLawyerByAdmin([FromBody]AcceptLawyer vm)
        {
            return await APIResponse(async () => {
                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await userRepository.AcceptRejectLawyerByAdmin(vm);

                return Ok(apiResponse);
            });
        }

        [SwaggerOperation(Summary = "Get all user from here by Ispending and RoleId")]
        [HttpGet("GetAllUser")]
        public async Task<ActionResult> GetAllUser(bool IsPending, int RoleId)
        {
            return await APIResponse(async () => {
                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await userRepository.GetAllUser(IsPending, RoleId);

                return Ok(apiResponse);
            });
        }
        [SwaggerOperation(Summary = "Delete any user by id")]
        [HttpPost("DeleteUserByAdmin")]
        public async Task<ActionResult> DeleteUser([Required]long loggedInUser,[Required]long UserId)
        {
            return await APIResponse(async () => {
                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await userRepository.DeleteUser(loggedInUser, UserId);

                return Ok(apiResponse);
            });
        }
        [SwaggerOperation(Summary = "User Count By Role Type -1 For All")]
        [HttpPost("UserCountByRoleType")] 
        public async Task<ActionResult> UserCountByRoleType([Required] int roleType)
        {
            return await APIResponse(async () => { 
                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await userRepository.UserCountByRoleType(roleType);

                return Ok(apiResponse);
            });
        }

        [SwaggerOperation(Summary = "User Count By Approved/Pending Status -1 For All")]
        [HttpPost("UserCountByApprovedPendingStatus")]
        public async Task<ActionResult> UserCountByApprovedPendingStatus([Required] int roleType) 
        {
            return await APIResponse(async () => {
                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await userRepository.UserCountByApprovedUnApprovedStatus(roleType);

                return Ok(apiResponse);
            });
        }

        [SwaggerOperation(Summary = "Create my diary notification for user- any user can use this")]
        [HttpPost("CreateDiaryReminder")]
        public async Task<ActionResult> CreateDiaryReminder([Required]string text,[Required]DateTime dateTime)
        {
            return await APIResponse(async () => {
                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = await userRepository.CreateDiaryReminder(text,dateTime);

                return Ok(apiResponse);
            });
        }
    }
}
