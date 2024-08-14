using Business.Services;
using Business.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SignalRSwaggerGen.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Chat_Hub
{
    [SignalRHub]
    [Authorize]
    public sealed class ChatHub : Hub<IChatHub>
    {
        private INotificationRepository _notificationService;
        private IMessageRepository _messageService;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<Hub<IChatHub>> _logger;
        private readonly IHubContext<ChatHub, IChatHub> _context;

        public ChatHub(IServiceProvider serviceProvider, ILogger<Hub<IChatHub>> logger, IHubContext<ChatHub, IChatHub> context)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _context = context;
        }
        public override async Task OnConnectedAsync()
        {
            try
            {
                var C = Context.ConnectionId;
                _logger.LogError("Connection Created connection id:" + C);

                //await SendMessage("3", "");

                await PushDataOnConnectionEstablished();

                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception in OnConnectedAsync:" + ex.Message);

                throw ex;
            }
        }
        //public async Task SendMessage(string id = "3", string msg = "")
        //{
        //    try
        //    {
        //        _logger.LogError("SendMessage called: with id=" + id + "   msg=" + msg);
        //        //  await _context.Clients.All.DirectMessage(new MessageVm() { Content= msg });
        //        //await _context.Clients?.User(id).DirectMessage(msg)!;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("Exception in OnConnectedAsync:" + ex.Message);

        //        throw ex;
        //    }
        //}
        //public async Task SendMessageAsync()
        //{
        //    try
        //    {
        //        _logger.LogError("SendMessageAsync called");
        //        await PushDataOnConnectionEstablished();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("Exception in SendMessageAsync:" + ex.Message);
        //        throw ex;
        //    }
        //}

        private async Task PushDataOnConnectionEstablished()
        {
            var _userId = Context.UserIdentifier != null ? Convert.ToInt64(Context.UserIdentifier!) : -1;

            using (var scope = _serviceProvider.CreateScope())
            {
                _messageService = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
                _notificationService = scope.ServiceProvider.GetRequiredService<INotificationRepository>();

                var unReadNotification = await _notificationService.GetAllUnReadNotification(_userId);
                var unReadMessages = await _messageService.GetUnReadMessages(_userId);


                await UnReadMessage(unReadMessages, unReadMessages.Count(), _userId.ToString());
                await UnReadNotification(unReadNotification, unReadNotification.Count(), _userId.ToString());

            }
        }
        public async Task PushNotifications(long UserId)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                _notificationService = scope.ServiceProvider.GetRequiredService<INotificationRepository>();

                var unReadNotification = await _notificationService.GetAllUnReadNotification(UserId);

                await UnReadNotification(unReadNotification, unReadNotification.Count(), UserId.ToString());

            }
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {


                _logger.LogError("OnDisConnected :");
            }
            catch (Exception ex)
            {

                //throw ex;
            }
            return base.OnDisconnectedAsync(exception);
        }


        /// <summary>
        /// Delete Message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="toWho"></param>
        /// <returns></returns>
        public async Task DeleteMessage(string message, string toWho)
        {
            await Clients.User(toWho).DeleteMessage(message, toWho);
        }
        public async Task DirectMessage(MessageVm message)
        {
            try
            {
                if (Clients != null && message != null && message.ToUserId != null)
                {
                    // await _context.Clients.All.DirectMessage(message);
                    await _context.Clients?.User(message.ToUserId.ToString()).DirectMessage(message)!;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("DirectMessage Exception :" + ex.Message);
                //throw ex;
            }

        }
        public async Task UnReadMessage(List<MessageVm> messages, int count, string toWhom)
        {
            await Clients.User(toWhom).UnReadMessage(messages, count);
        }

        public async Task UnReadNotification(List<NotificationVm> notifications, int count, string toWhom)
        {
            await Clients.User(toWhom).UnReadNotification(notifications, count);
        }
        public async Task SendNotificationToClient(NotificationVm notification, int count, long userId)
        {
            await _context.Clients?.User(userId.ToString()).BroadcastNotification(notification, count);
        }
        //public async Task BroadcastNotification(NotificationVm notification, int count)
        //{
        //    await Clients.User(notification.ToUserId.ToString()).BroadcastNotification(notification, count);
        //}
    }
}
