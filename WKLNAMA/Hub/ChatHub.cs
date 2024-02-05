using Business.Services;
using Business.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace WKLNAMA.AppHub
{
    [SignalRHub]
   // [Authorize]
    public sealed class ChatHub: Hub<IChatHub>
    { 
        public override async Task  OnConnectedAsync()
        {
            //await Clients.All.DirectMessage("RecieveMessage");
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
            var d = Context.UserIdentifier;

            await Clients.User(message.ToUserId.ToString()).DirectMessage(message);
        }
    }
}
