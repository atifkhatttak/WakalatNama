using Business.Services;
using Business.ViewModels;
using Google.Apis.Drive.v3.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace WKLNAMA.AppHub
{
    [SignalRHub]
   // [Authorize]
    public sealed class ChatHub: Hub<IChatHub>
    {
        private  INotificationRepository _notificationService;
        private   IMessageRepository _messageService;
        private  readonly IServiceProvider _serviceProvider;

        public ChatHub(IServiceProvider serviceProvider)//
            //INotificationRepository notificationService, IMessageRepository messageService)
        {
            _serviceProvider = serviceProvider;
        }
        public override async Task  OnConnectedAsync()
        {
            var _userId = Context.UserIdentifier != null ? Convert.ToInt64(Context.UserIdentifier!) : -1;

            using ( var scope = _serviceProvider.CreateScope())
            {
               _messageService =  scope.ServiceProvider.GetRequiredService<IMessageRepository>();
               _notificationService = scope.ServiceProvider.GetRequiredService<INotificationRepository>();

                var unReadNotification = _notificationService.GetAllUnReadNotification(_userId);
                var unReadMessages = _messageService.GetUnReadMessages(_userId);

                Task.WaitAll(unReadMessages, unReadNotification);

                await UnReadMessage(unReadMessages.Result,unReadMessages.Result.Count(), _userId.ToString());
                await UnReadNotification(unReadNotification.Result, unReadNotification.Result.Count(), _userId.ToString());

            }
            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

 
        /// <summary>
        /// Delete Message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="toWho"></param>
        /// <returns></returns>
        public async Task DeleteMessage(string message,string toWho) 
        {
            await Clients.User(toWho).DeleteMessage( message,toWho);
        }
        public async Task DirectMessage(MessageVm message)
        {
            await Clients.User(message.ToUserId.ToString()).DirectMessage(message);
        }
        public async Task UnReadMessage(List<MessageVm> messages, int count, string toWhom)
        {
            await Clients.User(toWhom).UnReadMessage(messages, count);
        }
         
        public async Task UnReadNotification(List<NotificationVm> notifications, int count,string toWhom )
        {
            await Clients.User(toWhom).UnReadNotification(notifications,count); 
        }

    }
}
