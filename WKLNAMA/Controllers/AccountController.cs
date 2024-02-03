using Business.BusinessLogic;
using Business.Services;
using Business.ViewModels;
using Data.DomainModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WKLNAMA.Models;

namespace WKLNAMA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
     public class AccountController : BaseController<AppUser>
    {
        private readonly IAccountRepository accountRepository;
        private readonly IDocumentService documentService;

        private ApiResponse apiResponse = new ApiResponse();
        public AccountController(IAccountRepository accountRepository, IHttpContextAccessor httpContextAccessor, IDocumentService documentService) :base(accountRepository,httpContextAccessor)
        {
            this.accountRepository = accountRepository;
            this.documentService = documentService;
        }
        [AllowAnonymous]
        [HttpPost("Register")]
        public async   Task<ActionResult> Post([FromForm]RegisterViewModel _viewModel)
        {
            try
            {
                var result= await accountRepository.Register(_viewModel);
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

 
        //


        [AllowAnonymous]
        [HttpPost("SignIn")]
        public async Task<ActionResult> Post(LoginViewModel _viewModel)
        {
            try
            {
                var token = await accountRepository.SignInUser(_viewModel);

                apiResponse.Message = HttpStatusCode.Created.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.Created;
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
    }
}
