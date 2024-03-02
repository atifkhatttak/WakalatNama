using Business.Services;
using Data.DomainModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WKLNAMA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : BaseController<Notification>
    {
        private readonly INotificationRepository _notificationService;

        public NotificationController(INotificationRepository notificationService, IHttpContextAccessor httpContextAccessor, ILogger<NotificationController> logger): base(notificationService, httpContextAccessor, logger)
        {
            _notificationService = notificationService;
        }
    }
}
