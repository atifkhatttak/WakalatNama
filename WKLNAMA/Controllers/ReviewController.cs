using Business.BusinessLogic;
using Business.Services;
using Data.DomainModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjWakalatnama.DataLayer.Models;
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
        public async Task<ActionResult> GetUserReview(int? UserId)
        {
            try
            {
                var result = await reviewRepository.GetUserReviews(UserId);
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
