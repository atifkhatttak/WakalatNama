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
    public class MessageRepository : BaseRepository<Message>, IMessageRepository
    {
        public MessageRepository(WKNNAMADBCtx ctx) : base(ctx)
        {
            Ctx = ctx;
        }

        public WKNNAMADBCtx Ctx { get; }

        public async Task<MessageVm> Create(MessageVm message)
        {
            var _message = new Message
            {
                Id = message.Id,
                Content = message.Content,
                FromUserId = message.FromUserId,
                ToUserId = message.ToUserId,
                IsRead = message.IsRead

            };

            Insert(_message);
            await SaveAsync();

            message.Id = _message.Id;

            return message;
        }

        public async Task<List<MessageVm>> GetPrivateChat(MessageVm userDetails)
        {
            var messages = await Ctx.Messages.Where(x => (x.FromUserId == userDetails.FromUserId || x.ToUserId == userDetails.FromUserId) && (x.FromUserId == userDetails.ToUserId || x.ToUserId == userDetails.ToUserId))
                  .Select(x => new MessageVm { Id = x.Id, Content = x.Content, FromUserId = x.FromUserId, ToUserId = x.ToUserId, IsRead = x.IsRead, ParentId = x.ParentId })
                  .ToListAsync();

            return messages;
        }

        //public async Task<MessageWrapper> GetMessages(long userId)
        //{
        //  var _unreadMessagesByUser  = await  Ctx.Messages.Where(x => x.ToUserId == userId).GroupBy(x => x.FromUserId).Select(x => new {
        //                                   UserId= x.Key,
        //                                   UnReadMessages= x.Count(),
        //                                   LastMessage= x.LastOrDefault()
        //                                   }).ToListAsync(); 
           
        //}

        public async Task<List<MessageVm>> GetUnReadMessages(long userId)
        {
          return await  Ctx.Messages.Where(x => x.ToUserId == userId && x.IsRead != true).Select(x => new MessageVm
            {
                Id=x.Id,
                FromUserId=x.FromUserId,
                ToUserId=x.ToUserId,
                Content=x.Content,
                ParentId=x.ParentId,
                IsRead=x.IsRead
            }).ToListAsync();
        }

        public async Task<int> GetAllUnReadMessagesCount(long userId) 
        { 
            return await Ctx.Messages.Where(x => x.ToUserId == userId && x.IsRead != true).CountAsync();
        }

        public async Task<int> GetUnReadMessagesCountByUser(MessageVm messageVm) 
        {
            return await Ctx.Messages.Where(x => x.ToUserId == messageVm.ToUserId && x.FromUserId==messageVm.FromUserId && x.IsRead != true).CountAsync();
        }

        public async Task MarkRead(List<MessageVm> messages) 
        {
            foreach (var message in messages)
            {
                Message _message = new Message { Id = message.Id, Content = message.Content, FromUserId = message.FromUserId, IsRead = message.IsRead,ToUserId=message.ToUserId,ParentId=message.ParentId };
                 Ctx.Messages.Entry(_message).State = EntityState.Modified;
            }
            
          await  Ctx.SaveChangesAsync();
        }

    }
}
