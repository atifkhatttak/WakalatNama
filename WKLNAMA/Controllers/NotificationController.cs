using Business.Services;
using Data.DomainModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WKLNAMA.AppHub;

namespace WKLNAMA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : BaseController<Notification>
    {
        private readonly INotificationRepository _notificationService;
        private readonly ChatHub _chatHub;

        public NotificationController(INotificationRepository notificationService, IHttpContextAccessor httpContextAccessor, ILogger<NotificationController> logger, ChatHub chatHub): base(notificationService, httpContextAccessor, logger)
        {
            _notificationService = notificationService;
            _chatHub = chatHub;
        }

        [HttpPost("MarkRead")]
        public async Task<ActionResult> MarkRead(List<long> ids)
        {
            return await APIResponse( async () => {

                await _notificationService.MarkIsRead(ids);

                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = "Messages have been marked as Read";

                return Ok(apiResponse);
            });
        }
    }
}
