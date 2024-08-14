using Business.Services;
using Business.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net;
using System.Security.Claims;
using WKLNAMA.Models;

namespace WKLNAMA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomBaseController : ControllerBase
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger _logger;
        protected ApiResponse apiResponse = new ApiResponse();
        public CustomBaseController(IHttpContextAccessor httpContextAccessor, ILogger logger = null)
        {
            this.httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }


        public UserIdentityModelVm UserModel => new UserIdentityModelVm()
        {
            UserId = Convert.ToInt64(string.IsNullOrEmpty(User.FindFirst("UserId").Value) ? 0 : User.FindFirst("UserId").Value),
            FirstName = User.FindFirstValue("FirstName")!,
            LastName = User.FindFirstValue("LastName")!,
            Role = User.FindFirstValue("Role")!,
            Email = User.FindFirstValue("Email")!,
            UserName = User.FindFirstValue("UserName")!
        };


        [NonAction]
        public async Task<ActionResult> APIResponse(Func<Task<ActionResult>> callback)
        {
            try
            {

                return await callback();
            }
            catch (Exception ex)
            {
                apiResponse.Message = "Internal server error occured,Please contact you adminnistrator!";
                apiResponse.HttpStatusCode = HttpStatusCode.InternalServerError;
                apiResponse.Success = false;
                apiResponse.Data = null;
                _logger.LogError(ex.Message);
                return Ok(apiResponse);
            }
        }
    }
}
