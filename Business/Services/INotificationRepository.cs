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
        public Task<List<NotificationVm>> GetAllNotificationByUser(long UserId);   
        public Task<List<Notification>> MarkIsRead(List<long> notificationIds);
        public Task<Notification> AddNotification(NotificationVm model);
        public Task BroadCastAndSaveNotification(NotificationVm model);
        Task<bool> MarkAllNotificationRead(long UserId);
        Task<bool> MarkSingleNotification(long UserId,long NotificationId); 
        Task SendEmailScheduledNotification(List<FetchCaseDateNotificationDataVm> userData, CancellationToken cancellationToken);
        Task<bool> SendPushNotification(long userId,string title,string body);
        Task<T> SendInstantNotification<T>(T requestObject, int notificationMedium, int notificationType, CancellationToken stoppingToken);
        Task SendSmsSchedledNotification(List<FetchCaseDateNotificationDataVm> userData, CancellationToken cancellationToken);
    }
}
