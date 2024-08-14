using Business.Chat_Hub;
using Business.Services;
using Data.DomainModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
 
namespace WKLNAMA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
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
        [HttpGet("AllNotificationByUser")]
        public async Task<ActionResult> GetUserAllNotification() 
        {
            return await APIResponse(async () => {
                if (UserModel.UserId>0)
                {
                 var notifications =  await _notificationService.GetAllNotificationByUser(UserModel.UserId);

                    apiResponse.Message = HttpStatusCode.OK.ToString();
                    apiResponse.HttpStatusCode = HttpStatusCode.OK;
                    apiResponse.Success = true;
                    apiResponse.Data = notifications;
                }
                return Ok(apiResponse);
            });
        }
        [HttpGet("AllUnReadNotificationByUser")]
        public async Task<ActionResult> AllUnReadNotificationByUser() 
        {
            return await APIResponse(async () => {
                if (UserModel.UserId > 0)
                {
                 var unreadNotification =   await _notificationService.GetAllUnReadNotification(UserModel.UserId);

                    apiResponse.Message = HttpStatusCode.OK.ToString();
                    apiResponse.HttpStatusCode = HttpStatusCode.OK;
                    apiResponse.Success = true;
                    apiResponse.Data = unreadNotification;
                }
                return Ok(apiResponse);
            });
        }

        [HttpPost("MarkUserAllNotification")]
        public async Task<ActionResult> MarkUserAllNotification()
        {
            return await APIResponse(async () => {
                if (UserModel.UserId > 0)
                {
                    var notifications = await _notificationService.MarkAllNotificationRead(UserModel.UserId);

                    apiResponse.Message = HttpStatusCode.OK.ToString();
                    apiResponse.HttpStatusCode = HttpStatusCode.OK;
                    apiResponse.Success = true;
                    apiResponse.Data = notifications;
                }
                return Ok(apiResponse);
            });
        }
        [HttpPost("MarkSingleNotificationRead")]
        public async Task<ActionResult> MarkSingleNotificationRead([Required]long NotificationId)
        {
            return await APIResponse(async () => {
                if (UserModel.UserId > 0)
                {
                    var notifications = await _notificationService.MarkSingleNotification(UserModel.UserId, NotificationId);

                    apiResponse.Message = HttpStatusCode.OK.ToString();
                    apiResponse.HttpStatusCode = HttpStatusCode.OK;
                    apiResponse.Success = true;
                    apiResponse.Data = notifications;
                }
                return Ok(apiResponse);
            });
        }

        [HttpPost("SendNotification")]
        public async Task<ActionResult> SendNotification(long userId, string body, string title)
        {
            _notificationService.SendPushNotification(userId, title,body);
            return await APIResponse(async () =>
            {
                //if (UserModel.UserId > 0)
                //{

                apiResponse.Message = HttpStatusCode.OK.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.OK;
                apiResponse.Success = true;
                apiResponse.Data = true;
                //}
                return Ok(apiResponse);
            });
        }
    }
}
