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
    public class MessageRepository : BaseRepository<Message>,IMessageRepository
    {
        public MessageRepository(WKNNAMADBCtx ctx):base(ctx)
        {
            Ctx = ctx;
        }

        public WKNNAMADBCtx Ctx { get; }

        public async Task<MessageVm> Create(MessageVm message) 
        {
            var _message = new Message
            {
                Id=message.Id,
                Content=message.Content,
                FromUserId=message.FromUserId,
                ToUserId=message.ToUserId,
                IsRead=message.IsRead

            };

           Insert(_message);
          await  SaveAsync();

            message.Id = _message.Id;

            return message;
        }

        public async Task<List<MessageVm>> GetPrivateChat(MessageVm userDetails)
        {
          var messages=  await Ctx.Messages.Where(x => ( x.FromUserId == userDetails.FromUserId ||  x.ToUserId==userDetails.FromUserId) && (x.FromUserId==userDetails.ToUserId || x.ToUserId==userDetails.ToUserId))
                .Select(x=> new MessageVm { Id = x.Id, Content = x.Content, FromUserId = x.FromUserId, ToUserId = x.ToUserId, IsRead = x.IsRead, ParentId=x.ParentId })                           
                .ToListAsync();

            return messages;
        }
    }
}
