using Business.BusinessLogic;
using Business.Helpers.Attributes;
using Business.Services;
using Business.ViewModels;
using Data.DomainModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using WKLNAMA.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WKLNAMA.Controllers
{
   // [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController<TEntity> : ControllerBase where TEntity : class
    {
        protected IBaseRepository<TEntity> _baseRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger<BaseController<TEntity>> _logger;
        protected ApiResponse apiResponse = new ApiResponse();

        public UserIdentityModelVm UserModel => new UserIdentityModelVm()
                {
                    UserId = Convert.ToInt64(string.IsNullOrEmpty(User.FindFirst("UserId").Value) ? 0 : User.FindFirst("UserId").Value),
                    FirstName = User.FindFirstValue("FirstName")!,
                    LastName = User.FindFirstValue("LastName")!,
                    Role = User.FindFirstValue("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")!,
                    Email = User.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")!,
                    UserName = User.FindFirstValue("UserName")!
                };
        
        public BaseController(IBaseRepository<TEntity> baseRepository, IHttpContextAccessor httpContextAccessor, ILogger<BaseController<TEntity>> logger=null)
        {
            _baseRepository = baseRepository;
            this.httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        [HttpGet]
        public virtual async Task<ActionResult> GetAll()
        {
            try
            {
                var data = await _baseRepository.GetAll();
                apiResponse.Success = true;
                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Data = data;
            }
            catch (Exception ex)
            {
                apiResponse.Success = false;
                apiResponse.Message = ex.Message;
                apiResponse.HttpStatusCode = HttpStatusCode.InternalServerError;
                apiResponse.Data = null;
            }


            return Ok(apiResponse);
        }
        [HttpPost]
        public async virtual Task<ActionResult> Post(TEntity _viewModel)
        {
            try
            {
                _baseRepository.Insert(_viewModel);
                await _baseRepository.SaveAsync();
                apiResponse.Success = true;
                apiResponse.Message = HttpStatusCode.Created.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.Created;
                apiResponse.Data = _viewModel;
            }
            catch (Exception ex)
            {
                apiResponse.Success = false;
                apiResponse.Message = ex.Message;
                apiResponse.HttpStatusCode = HttpStatusCode.InternalServerError;
                apiResponse.Data = null;
            }
            return Ok(apiResponse);
        }

        [HttpGet("Find")]
        public async virtual Task<ActionResult> GetById(object id)
        {
            try
            {
                var data = await _baseRepository.GetById(id);
                apiResponse.Success = true;
                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Data = data;
            }
            catch (Exception ex)
            {
                apiResponse.Success = false;
                apiResponse.Message = ex.Message;
                apiResponse.HttpStatusCode = HttpStatusCode.InternalServerError;
                apiResponse.Data = null;
            }
            return Ok(apiResponse);
        }

        [HttpDelete]
        public async virtual Task<ActionResult> Delete(object id)
        {
            try
            {
                await _baseRepository.Delete(Convert.ToInt32(id.ToString()));
                await _baseRepository.SaveAsync();

                apiResponse.Success = true;
                apiResponse.Message = "Recored Deleted";
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Data = id.ToString();
            }
            catch (Exception ex)
            {
                apiResponse.Success = false;
                apiResponse.Message = ex.Message;
                apiResponse.HttpStatusCode = HttpStatusCode.InternalServerError;
                apiResponse.Data = null;
            }
            return Ok(apiResponse);
        }

        [HttpPut]
        public async virtual Task<ActionResult> Update(TEntity _object)
        {
            try
            {

                _baseRepository.Update(_object);
               var t= _baseRepository.SaveAsync();
                t.Wait();

                if (t.IsCompletedSuccessfully)
                {
                    apiResponse.Success = true;
                    apiResponse.Message = "Recored Updated";
                    apiResponse.HttpStatusCode = HttpStatusCode.OK;
                    apiResponse.Data = _object;
                }
            }
            catch (Exception ex)
            {
                apiResponse.Success = false;
                apiResponse.Message = ex.Message;
                apiResponse.HttpStatusCode = HttpStatusCode.InternalServerError;
                apiResponse.Data = null;
            }
            return Ok(apiResponse);
        }

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
