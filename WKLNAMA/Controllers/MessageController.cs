using Business.Services;
using Business.ViewModels;
using Data.DomainModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ChatHub _chatHub;

        public MessageController(IMessageRepository message, IHttpContextAccessor httpContextAccessor, ChatHub chatHub): base(message, httpContextAccessor)
        {
            _message = message;
            this.httpContextAccessor = httpContextAccessor;
            _chatHub = chatHub;
        }

        [HttpPost("GetPrivateChat")] 
        public async Task<ActionResult> GetPrivateChat(MessageVm userDetails) 
        {
            if (userDetails.FromUserId == UserModel.UserId) 
            {
                var messages = await _message.GetPrivateChat(userDetails);
                return Ok(messages);
            }
            return BadRequest();
        }


        [HttpPost("Send")]
        public async  Task<ActionResult> Send(MessageVm message)
        {

           await _message.Create(message);

            await _chatHub.Clients.User(message.ToUserId.ToString()).DirectMessage(message);

            return NoContent();
            
        }
        [HttpPost("Delete")]
        public async Task<ActionResult> Delete(MessageVm message)
        {
            await _message.Delete(message.Id);
            await _message.SaveAsync();

            await _chatHub.Clients.User(message.FromUserId.ToString()).DeleteMessage( "Message has bee deleted", message.Id.ToString());

            return NoContent();
        }

        [HttpPost("Reply")]
        public async Task<ActionResult> Reply(MessageVm message)
        {
             
            await _message.Create(message);
            await _message.SaveAsync();

            await _chatHub.Clients.User(message.ToUserId.ToString()).DirectMessage(message);

            return NoContent();
        }

        #region Hide Override Methods

        [NonAction]
        public override async Task<ActionResult> GetAll() => await Task.FromResult(NoContent());

        [NonAction]
        public override async Task<ActionResult> Post(Message T) => await Task.FromResult(NoContent());
        [NonAction]
        public async override Task<ActionResult> GetById(Object _viewModel) =>  await Task.FromResult(NoContent());

        [NonAction]
        public async override Task<ActionResult> Delete(object id) => await Task.FromResult( NoContent());
        [NonAction]
        public async override Task<ActionResult> Update(Message _object)=> await Task.FromResult(NoContent());
        #endregion

    }
}
