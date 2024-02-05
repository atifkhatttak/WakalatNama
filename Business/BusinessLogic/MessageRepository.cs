using Business.Services;
using Business.ViewModels;
using Data.Context;
using Data.DomainModels;
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
                
        }

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

    }
}
