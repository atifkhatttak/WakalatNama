using Business.BusinessLogic;
using Business.Services;
using Business.ViewModels;
using Data.DomainModels;
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
    public class ReviewController : BaseController<Review>
    {
        private readonly IReviewRepository reviewRepository;

        private ApiResponse apiResponse = new ApiResponse();
        public ReviewController(IReviewRepository reviewRepository, IHttpContextAccessor httpContextAccessor, IDocumentService documentService) : base(reviewRepository, httpContextAccessor)
        {
            this.reviewRepository = reviewRepository;
        }

        [HttpGet("GetUserReview")]
        [SwaggerOperation(Summary = "Get Lawyer reviews list from here-secured")]
        public async Task<ActionResult> GetUserReview(int? UserId)
        {
                return await APIResponse(async () => {
                    apiResponse.Message = HttpStatusCode.OK.ToString();
                    apiResponse.HttpStatusCode = HttpStatusCode.OK;
                    apiResponse.Success = true;
                    apiResponse.Data = await reviewRepository.GetUserReviews(UserId);

                    return Ok(apiResponse);
                });
        }
        [HttpPost("MarkFaourite")]
        public async Task<ActionResult> MarkFaourite(ReviewFavouriteVM favourite)
        {
            try
            {
                await reviewRepository.MarkFaourite(favourite);
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

            //return Ok(Task.FromResult(_viewModel));
            return Ok(apiResponse);
        }
        [HttpPost("GetUserFavourites")]
        public async Task<ActionResult> GetUserFavourites(long? userId)
        {
            try
            {
               var result= await reviewRepository.GetFavouriteLawyers(userId);
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

            //return Ok(Task.FromResult(_viewModel));
            return Ok(apiResponse);
        }
    }
}
