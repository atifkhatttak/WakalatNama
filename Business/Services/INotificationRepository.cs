using Business.ViewModels;
using Data.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public interface INotificationRepository: IBaseRepository<Notification>
    {
        public Task<List<NotificationVm>> GetAllUnReadNotification(long UserId);
        public Task<List<Notification>> MarkIsRead(List<long> notificationIds);
    }
}
