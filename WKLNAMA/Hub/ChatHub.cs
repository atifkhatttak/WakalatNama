using Business.Services;
using Business.ViewModels;
using Microsoft.AspNetCore.SignalR;
 
namespace WKLNAMA.AppHub
{
    public sealed class ChatHub: Hub<IChatHub>
    { 
        public override async Task  OnConnectedAsync()
        {
            await Clients.All.DirectMessage("RecieveMessage");
        }

        //public async Task DirectMessage(string message) 
        //{
        //    await Clients.All.DirectMessage("RecieveMessage",message);
        //}
        public async Task DirectMessage(MessageVm message)
        {
            await Clients.All.DirectMessage("RecieveMessage", message);
        }
    }
}
