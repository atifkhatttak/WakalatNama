using Business.Services;
using Business.ViewModels;
using Data.Context;
using Data.DomainModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.BusinessLogic
{
    public class NotificationRepository: BaseRepository<Notification>, INotificationRepository
    {
        public WKNNAMADBCtx _ctx { get; }

        public NotificationRepository(WKNNAMADBCtx ctx): base(ctx) 
        {
            _ctx = ctx;
        }

        public async Task<List<NotificationVm>> GetAllUnReadNotification(long UserId)
        {
            return await  _ctx.Notifications.Where(x => x.ToUserId == UserId).Select(x=>new 
              NotificationVm 
            {  
                Content=x.Content,
                Id=x.Id,
                IsRead=x.IsRead,
                FromUserId=x.FromUserId,
                ToUserId=x.ToUserId,
                NotificationType=x.NotificationType,
                CreatedDate=x.CreatedDate
            }).ToListAsync();
        }

        public async Task<List<Notification>> MarkIsRead(List<long> notificationIds)
        {
            List<Notification> _notification = await _ctx.Notifications.Where(x => notificationIds.Contains(x.Id)).ToListAsync();
            
            if(_notification?.Count()>0)
            _notification.ForEach(x => x.IsRead = true);

            await _ctx.SaveChangesAsync();

            return _notification!;
        }
    }
}
