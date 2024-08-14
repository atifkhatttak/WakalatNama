using Business.Services;
using Business.ViewModels;
using Data.Context;
using Data.DomainModels;
using Google.Apis.Drive.v3.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
            Message _message = new Message();
            try
            {

                _message = new Message
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

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return message;
        }
        public async Task CreateChatSession(MessageVm message)
        {
            ChatSession sesison = new ChatSession();
            try
            {
             var res= await  Ctx.ChatSessions.Where(x => (x.SenderId == message.FromUserId && x.ReceiverId == message.ToUserId)
               || (x.SenderId == message.ToUserId && x.ReceiverId == message.FromUserId) && !x.IsDeleted)
                   .ExecuteUpdateAsync(s=>s.SetProperty(x=>x.LastMessageId,message.Id));

                if (res > 0) return;

                sesison = new ChatSession
                {
                    LastMessageId = message.Id,
                    SenderId = message.FromUserId,
                    ReceiverId = message.ToUserId
                };

               await Ctx.AddAsync(sesison);
              await  Ctx.SaveChangesAsync();

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public async Task<List<MessageVm>> GetPrivateChat(MessageVm userDetails)
        {
            var messages = await Ctx.Messages.Where(x => (x.FromUserId == userDetails.FromUserId || x.ToUserId == userDetails.FromUserId) && (x.FromUserId == userDetails.ToUserId || x.ToUserId == userDetails.ToUserId))
                  .Select(x => new MessageVm { Id = x.Id, Content = x.Content, FromUserId = x.FromUserId, ToUserId = x.ToUserId, IsRead = x.IsRead, ParentId = x.ParentId,DateTime=x.CreatedDate })
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
            return await Ctx.Messages.Where(x => x.ToUserId == userId && x.IsRead != true).Select(x => new MessageVm
            {
                Id = x.Id,
                FromUserId = x.FromUserId,
                ToUserId = x.ToUserId,
                Content = x.Content,
                ParentId = x.ParentId,
                IsRead = x.IsRead
            }).ToListAsync();
        }

        public async Task<int> GetAllUnReadMessagesCount(long userId)
        {
            return await Ctx.Messages.Where(x => x.ToUserId == userId && x.IsRead != true).CountAsync();
        }

        public async Task<int> GetUnReadMessagesCountByUser(MessageVm messageVm)
        {
            return await Ctx.Messages.Where(x => x.ToUserId == messageVm.ToUserId && x.FromUserId == messageVm.FromUserId && x.IsRead != true).CountAsync();
        }

        public async Task MarkRead(List<MessageVm> messages)
        {
            foreach (var message in messages)
            {
                Message _message = new Message { Id = message.Id, Content = message.Content, FromUserId = message.FromUserId, IsRead = message.IsRead, ToUserId = message.ToUserId, ParentId = message.ParentId };
                Ctx.Messages.Entry(_message).State = EntityState.Modified;
            }

            await Ctx.SaveChangesAsync();
        }
        public async Task<bool> MarkRead(long SenderId,long RecieverId)
        {
            return await Ctx.Messages
             .Where(x => (x.FromUserId == SenderId || x.ToUserId == SenderId) && !x.IsRead && !x.IsDeleted)
     .ExecuteUpdateAsync(setters => setters.SetProperty(m => m.IsRead, true)) > 0;
        }

        public async Task<List<ChatHistoryVM>> GetChatHistory(long FromUserId, long ToUserId)
        {
            var ChatHistory = await (from m in Ctx.Messages
                           join u1 in Ctx.UserProfiles on m.FromUserId equals u1.UserId
                           join u2 in Ctx.UserProfiles on m.ToUserId equals u2.UserId
                           where m.FromUserId == FromUserId && m.ToUserId == ToUserId
                           && !u1.IsDeleted && !u2.IsDeleted && !m.IsDeleted
                           select new ChatHistoryVM
                           {

                               Id = m.Id,
                               Content = m.Content,
                               FromUserName = u1.FullName,
                               ToUserName = u2.FullName,
                               FromUserId = m.FromUserId,
                               ToUserId = m.ToUserId,
                               IsRead = m.IsRead,
                               ParentId = m.ParentId,
                               CreatedDate = m.CreatedDate,
                               UpdateDate = m.UpdateDate,
                               IsDeleted = m.IsDeleted,
                           }).ToListAsync();

            return ChatHistory;

        }

        public async Task<List<ChatBoxVm>> GetChatBoxData(long UserId)
        {
            List<ChatBoxVm> chatBoxData = new List<ChatBoxVm>();
            try
            {

                chatBoxData = await Ctx.Database.SqlQueryRaw<ChatBoxVm>($@"
                               SELECT 
                                       UserId,
                                      LatestMessage.Mes_Id AS MessageId,
                                      FullName,
                                      Content,
                                      Messages.CreatedDate,
                                      IsRead,
                                      ISNULL(ProfilePicUrl, '') AS ImageUrl,
                               	   LatestMessage.UnRead AS UnReadCount
                               FROM dbo.Messages
                                   INNER JOIN
                                   (
                                       SELECT CASE
                                                  WHEN FromUserId != @UserId THEN
                                                      FromUserId
                                                  ELSE
                                                      ToUserId
                                              END AS GUserId,
                                              MAX(Id) AS Mes_Id,
                               			   SUM( CASE WHEN IsRead =0 THEN 1 ELSE 0 end) AS UnRead
                                       FROM dbo.Messages
                                           INNER JOIN dbo.UserProfiles
                                               ON FromUserId = UserId
                                                  OR ToUserId = UserId
                                       WHERE UserId = @UserId
                                       GROUP BY CASE
                                                    WHEN FromUserId != @UserId THEN
                                                        FromUserId
                                                    ELSE
                                                        ToUserId
                                                END
                                   ) LatestMessage
                                       ON Id = LatestMessage.Mes_Id
                                   INNER JOIN dbo.UserProfiles
                                       ON (
                                              UserId = ToUserId
                                              AND UserId != @UserId
                                          )
                                          OR
                                          (
                                              FromUserId = UserId
                                              AND UserId != @UserId
                                          )                                           
                		   ", new SqlParameter("@UserId", UserId))
                                .ToListAsync();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return chatBoxData;
        }
    }
}
