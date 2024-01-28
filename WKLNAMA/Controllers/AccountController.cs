using Business.BusinessLogic;
using Business.Services;
using Business.ViewModels;
using Data.DomainModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WKLNAMA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
     public class AccountController : BaseController<AppUser>
    {
        private readonly IAccountRepository accountRepository;

        public AccountController(IAccountRepository accountRepository, IHttpContextAccessor httpContextAccessor) :base(accountRepository,httpContextAccessor)
        {
            this.accountRepository = accountRepository;
            
        }
        [AllowAnonymous]
        [HttpPost("Register")]
        public async   Task<ActionResult> Post(RegisterViewModel _viewModel)
        {
           await  accountRepository.Register(_viewModel);
            return Ok(Task.FromResult(_viewModel));
        }

        [AllowAnonymous]
        [HttpPost("SignIn")]
        public async Task<ActionResult> Post(LoginViewModel _viewModel)
        {
          var token = await accountRepository.SignInUser(_viewModel);
            return Ok(Task.FromResult(token));
        }
    }
}
