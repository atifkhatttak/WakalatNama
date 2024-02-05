using Business.Services;
using Business.ViewModels;
using Data.DomainModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.CodeDom;
using System.Net;
using WKLNAMA.AppHub;
using WKLNAMA.Models;

namespace WKLNAMA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : BaseController<Message>
    {
        private readonly IMessageRepository _message;
        private readonly IHubContext<ChatHub> _hubContext;

        public MessageController(IMessageRepository message, IHttpContextAccessor httpContextAccessor, IHubContext<ChatHub> hubContext): base(message, httpContextAccessor)
        {
            _message = message;
            _hubContext = hubContext;
        }

        [NonAction]
        public async override Task<ActionResult> Post(Message _viewModel) { return NoContent(); }

        [HttpPost("Send")]
        public async  Task<ActionResult> Send(MessageVm message)
        {

           await _message.Create(message);

          await  _hubContext.Clients.All.SendAsync("RecievedMessage",message);

            return NoContent();
            
        }

    }
}
