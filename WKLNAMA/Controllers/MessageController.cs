using Business.Helpers.Attributes;
using Business.Services;
using Business.ViewModels;
using Data.DomainModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using System.CodeDom;
using System.Net;
using WKLNAMA.AppHub;
using WKLNAMA.Models;

namespace WKLNAMA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : BaseController<Data.DomainModels.Message>
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

            return await APIResponse(async () =>
            {
                 apiResponse.Data = null;
                apiResponse.Message = HttpStatusCode.NotFound.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.NotFound;

                if (userDetails.FromUserId == UserModel.UserId)
                {
                    var messages = await _message.GetPrivateChat(userDetails);
                    apiResponse.Message = HttpStatusCode.OK.ToString();
                    apiResponse.HttpStatusCode = HttpStatusCode.OK;
                    apiResponse.Success = true;
                    apiResponse.Data = messages;
                }

                return Ok(apiResponse);

            });
        }

        [HttpPost("MarkRead")]
        public async Task<ActionResult> MarkMessagesRead(List<MessageVm> messages) 
        {
            return await APIResponse(async () =>
            {
                apiResponse.Data = null;
                apiResponse.Message = HttpStatusCode.NotFound.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.NotFound;

                if (messages?.FirstOrDefault()?.ToUserId == UserModel.UserId)
                {
                    await _message.MarkRead(messages);

                    apiResponse.Message = HttpStatusCode.OK.ToString();
                    apiResponse.HttpStatusCode = HttpStatusCode.OK;
                    apiResponse.Success = true;
                    apiResponse.Data = "Messages has been marked as read";
                }
                return Ok(apiResponse);
            });
        }


        [HttpPost("GetAllUnReadMessageCount")] 
        public async Task<ActionResult> GetAllUnReadMessageCount(long userId)
        {
          return  await APIResponse(async () => {

              apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
              apiResponse.Message = HttpStatusCode.BadRequest.ToString();

              if (userId == UserModel.UserId)
              {
                var messageCount= await _message.GetAllUnReadMessagesCount(userId);
                  apiResponse.Message = HttpStatusCode.OK.ToString();
                  apiResponse.HttpStatusCode = HttpStatusCode.OK;
                  apiResponse.Success = true;
                  apiResponse.Data = messageCount;
              }
               return Ok(apiResponse);
          });
           
        }
        [HttpPost("GetUnReadMessageCountByUser")]
        public async Task<ActionResult> GetUnReadMessageCountByUser(MessageVm messages)
        {
          return  await APIResponse(async () => 
            {
                apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                apiResponse.Message = HttpStatusCode.BadRequest.ToString();

                if (messages?.ToUserId == UserModel.UserId)
                {
                  var userUnreadMessages=   await _message.GetUnReadMessagesCountByUser(messages);

                    apiResponse.Message = HttpStatusCode.OK.ToString();
                    apiResponse.HttpStatusCode = HttpStatusCode.OK;
                    apiResponse.Success = true;
                    apiResponse.Data = userUnreadMessages;
                }
                return Ok(apiResponse);
            });

        }






        [HttpPost("Send")]
        public async  Task<ActionResult> Send(MessageVm message)
        {
            return await APIResponse(async () =>
            {
                apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                apiResponse.Message = HttpStatusCode.BadRequest.ToString();

                if (message?.ToUserId == UserModel.UserId)
                {
                    var messages = await _message.Create(message);

                    await _chatHub.Clients.User(message.ToUserId.ToString()).DirectMessage(message);

                    apiResponse.Message = HttpStatusCode.OK.ToString();
                    apiResponse.HttpStatusCode = HttpStatusCode.OK;
                    apiResponse.Success = true;
                    apiResponse.Data = messages;
                }
                return Ok(apiResponse);
            });
        }
        [HttpPost("Delete")]
        public async Task<ActionResult> Delete(MessageVm message)
        {
            return await APIResponse(async () => {

                apiResponse.Message = HttpStatusCode.BadRequest.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;

                if (message?.ToUserId == UserModel.UserId)
                {
                    await _message.Delete(message.Id);
                    await _message.SaveAsync();

                    apiResponse.Message = HttpStatusCode.OK.ToString();
                    apiResponse.HttpStatusCode = HttpStatusCode.OK;
                    apiResponse.Success = true;
                    apiResponse.Data = "Message has been deleted";

                    await _chatHub.Clients.User(message.FromUserId.ToString()).DeleteMessage("Message has bee deleted", message.Id.ToString());
                }
                return Ok(apiResponse);

            });

          
        }

        [HttpPost("Reply")]
        public async Task<ActionResult> Reply(MessageVm message)
        {
            return await APIResponse(async () => {


                apiResponse.Message = HttpStatusCode.BadRequest.ToString();
                apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;

                if (message?.ToUserId == UserModel.UserId)
                {
                  var _messageSent= await _message.Create(message);
                    await _message.SaveAsync();

                    await _chatHub.Clients.User(message.ToUserId.ToString()).DirectMessage(_messageSent);

                    apiResponse.Message = HttpStatusCode.OK.ToString(); 
                    apiResponse.HttpStatusCode = HttpStatusCode.OK;
                    apiResponse.Success = true;
                    apiResponse.Data = _messageSent;
                }
                return Ok(apiResponse);
            });
        }

        #region Hide Override Methods

        [NonAction]
        public override async Task<ActionResult> GetAll() => await Task.FromResult(NoContent());

        [NonAction]
        public override async Task<ActionResult> Post(Data.DomainModels.Message T) => await Task.FromResult(NoContent());
        [NonAction]
        public async override Task<ActionResult> GetById(Object _viewModel) =>  await Task.FromResult(NoContent());

        [NonAction]
        public async override Task<ActionResult> Delete(object id) => await Task.FromResult( NoContent());
        [NonAction]
        public async override Task<ActionResult> Update(Data.DomainModels.Message _object)=> await Task.FromResult(NoContent());
        #endregion

    }
}
