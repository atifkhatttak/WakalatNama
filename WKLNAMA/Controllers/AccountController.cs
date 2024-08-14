using Business.BusinessLogic;
using Business.Enums;
using Business.Services;
using Business.ViewModels;
using Data.DomainModels;
using Google.Apis.Drive.v3.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Net;
using WKLNAMA.Models;

namespace WKLNAMA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
     public class AccountController : BaseController<AppUser>
    {
        private readonly IAccountRepository accountRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDocumentService documentService;
        private readonly IEmailService _emailService;
        private ApiResponse apiResponse = new ApiResponse(); 
        public AccountController(IAccountRepository accountRepository, IHttpContextAccessor httpContextAccessor, IDocumentService documentService,IEmailService emailService) :base(accountRepository,httpContextAccessor)
        {
            this.accountRepository = accountRepository;
            _httpContextAccessor = httpContextAccessor;
            this.documentService = documentService;
            _emailService = emailService;
        }
        [AllowAnonymous]
        [HttpPost("Register")]
        public async   Task<ActionResult> Post([FromBody]RegisterViewModel _viewModel)
        {
            try
            {
                var result= await accountRepository.Register(_viewModel);
                if (result == null || result.UserId<=0)
                {
                    apiResponse.Message = HttpStatusCode.InternalServerError.ToString();
                    apiResponse.HttpStatusCode = HttpStatusCode.InternalServerError;
                    apiResponse.Success = false;
                    apiResponse.Data = null;
                }
                else
                {
                    apiResponse.Message = HttpStatusCode.Created.ToString();
                    apiResponse.HttpStatusCode = HttpStatusCode.Created;
                    apiResponse.Success = true;
                    apiResponse.Data = result;
                }
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


        [AllowAnonymous]
        [HttpPost("SignIn")]
        public async Task<ActionResult> Post(LoginViewModel _viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(_viewModel);
                }
                var token = await accountRepository.SignInUser(_viewModel);

                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = token;
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
        [AllowAnonymous]
        [HttpPost("GetUserClaims")]
        [SwaggerOperation(Summary = "Get user claims from here")]
        public async Task<ActionResult> GetClaims(LoginViewModel loginModel)
        {
                return await APIResponse(async () => {
                    apiResponse.Message = HttpStatusCode.OK.ToString();
                    apiResponse.HttpStatusCode = HttpStatusCode.OK;
                    apiResponse.Success = true;
                    apiResponse.Data = await accountRepository.GetClaims(loginModel);

                    return Ok(apiResponse);
                });
        }
        [AllowAnonymous]
        [HttpPost("CheckInstanSMSBackgroundTask")]
        [SwaggerOperation(Summary = "Get user claims from here")]
        public async Task<ActionResult> Check(CancellationToken stoppingToken)
        {
            return await APIResponse(async () => {
                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                 
                 new EmailInstantNotificationService(_httpContextAccessor?.HttpContext?.RequestServices).ExecuteAsync<FetchCaseDateNotificationDataVm>(new FetchCaseDateNotificationDataVm() { LawyerEmail="api1@yopmail.com",CitizenEmail="api2@yopmail.com"},(int)NotificationMedium.Email,(int)NotificationType.CaseDate, stoppingToken);

              //  await _smsInstant.ExecuteAsync<int>(1,CancellationToken.None);

                return Ok(apiResponse);
            });
        }

        [HttpPost("ForgotPassword")]
        public async Task<ActionResult> ForgotPassword([Required]string email)
        {
            try
            {
                var _appUser = await accountRepository.ForgotPassword(email);

                if(_appUser != null )
                {
                  await  _emailService.SendMailTrapEmail($"{_appUser?.FullName} | Password Reset", $"You OTP Code is: {_appUser?.OTPCode}", email);
                    apiResponse.Message = HttpStatusCode.OK.ToString();
                    apiResponse.HttpStatusCode = HttpStatusCode.OK;
                    apiResponse.Success = true;
                    apiResponse.Data = _appUser;
                }
                else{
                    apiResponse.Message = HttpStatusCode.NotFound.ToString();
                    apiResponse.HttpStatusCode = HttpStatusCode.NotFound;
                    apiResponse.Success = false;
                    apiResponse.Data = null;
                }

               
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

        [HttpPost("VerifyOTPToken")]
        public async Task<ActionResult> VerifyOTPToken([Required]string email, [Required] int otpCode)
        {
            try
            { 
                var _appUser = await accountRepository.VerifyOTP(email, otpCode);

                if (_appUser != null)
                {
                    await accountRepository.RemoveOTP(_appUser);
                    apiResponse.Message = HttpStatusCode.OK.ToString();
                    apiResponse.HttpStatusCode = HttpStatusCode.OK;
                    apiResponse.Success = true;
                    apiResponse.Data = _appUser;

                }
                else
                {
                    apiResponse.Message = HttpStatusCode.NotFound.ToString();
                    apiResponse.HttpStatusCode = HttpStatusCode.NotFound;
                    apiResponse.Success = false;
                    apiResponse.Data = null;
                }
               
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

        [HttpPost("ResetPassword")]
        public async Task<ActionResult> ResetPassword(AppUserVm resetPassword)
        {
            try
            {
                var _isPasswordReset = await accountRepository.ResetPassword(resetPassword);

                if (_isPasswordReset)
                {
                    apiResponse.Message = HttpStatusCode.OK.ToString();
                    apiResponse.HttpStatusCode = HttpStatusCode.OK;
                    apiResponse.Success = true;
                    apiResponse.Data = _isPasswordReset;
                }
                else
                {
                    apiResponse.Message = HttpStatusCode.NotFound.ToString();
                    apiResponse.HttpStatusCode = HttpStatusCode.NotFound;
                    apiResponse.Success = false;
                    apiResponse.Data = null;
                }

               
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

        [HttpPost("ChatUsers")]
        public async Task<ActionResult> GetChatUser([Required] long Id, [Required] string roleName)
        {
            try
            {
                List<AppUserVm> _chatUsers = new List<AppUserVm>();
                if (Id == UserModel.UserId)
                     _chatUsers = await accountRepository.GetChatUser(Id, roleName);

                    apiResponse.Message = HttpStatusCode.OK.ToString();
                    apiResponse.HttpStatusCode = HttpStatusCode.OK;
                    apiResponse.Success = true;
                    apiResponse.Data = _chatUsers;
                
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
        [HttpPost("UpdateDeviceToken")]
        public async Task<ActionResult> UpdateToken([FromBody]DeviceTokenVM vM)
        {
            return await APIResponse(async () =>
            {
                //if (UserModel.UserId == UserId)
                //{
                    apiResponse.Message = HttpStatusCode.OK.ToString();
                    apiResponse.HttpStatusCode = HttpStatusCode.OK;
                    apiResponse.Success = true;
                    apiResponse.Data =await accountRepository.UpdateToken(UserModel.UserId, vM.Token);
                //}
                return Ok(apiResponse);
            });
        }
    }
}
