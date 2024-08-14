using Business.BusinessLogic;
using Business.Enums;
using Business.Services;
using Business.ViewModels;
using Data.DomainModels;
using Google.Apis.Drive.v3.Data;
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
    public class PaymentController : BaseController<Payment>
    {
        private readonly IPaymentRepository paymentRepository;

        private ApiResponse apiResponse = new ApiResponse();
        public PaymentController(IPaymentRepository paymentRepository, IHttpContextAccessor httpContextAccessor) : base(paymentRepository, httpContextAccessor)
        {
            this.paymentRepository = paymentRepository;
        }

        [HttpPost("PostPayment")]
        [SwaggerOperation(Summary = "Use this api for payment")]
        public async Task<ActionResult> Payment(PaymentVM vM)
        {
                return await APIResponse(async () => {
                    apiResponse.Message = HttpStatusCode.OK.ToString();
                    apiResponse.HttpStatusCode = HttpStatusCode.OK;
                    apiResponse.Success = true;
                    apiResponse.Data = await paymentRepository.PostPayment(vM);

                    return Ok(apiResponse);
                });
        }
        [HttpGet("GetLawyerPayment")]
        [SwaggerOperation(Summary = "User this api to get all case payment by lawyer-lawyer side")]
        public async Task<ActionResult> GetLawyerPayment()
        {
            return await APIResponse(async () =>
            {
                if (UserModel.UserId > 0 && UserModel.Role==Roles.Lawyer.ToString())
                {
                    apiResponse.Message = HttpStatusCode.OK.ToString();
                    apiResponse.HttpStatusCode = HttpStatusCode.OK;
                    apiResponse.Success = true;
                    apiResponse.Data = await paymentRepository.GetLawyerPayment(UserModel.UserId);
                }
                return Ok(apiResponse);
            
            });
        }
        [HttpGet("GetLawyerCaseTransactions")]
        [SwaggerOperation(Summary = "get all transaction for case by lawyer - lawyer")]
        public async Task<ActionResult> GetLawyerCaseTransactions([Required]long CaseId)
        {
            return await APIResponse(async () =>
            {
                if (UserModel.UserId > 0 && UserModel.Role == Roles.Lawyer.ToString())
                {
                    apiResponse.Message = HttpStatusCode.OK.ToString();
                    apiResponse.HttpStatusCode = HttpStatusCode.OK;
                    apiResponse.Success = true;
                    apiResponse.Data = await paymentRepository.GetLawyerPaymentByCase(UserModel.UserId,CaseId);
                }
                return Ok(apiResponse);

            });
        }
    }
}
