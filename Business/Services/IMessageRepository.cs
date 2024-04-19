using Business.ViewModels;
using Data.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public interface IMessageRepository : IBaseRepository<Message>
    {
        public Task<MessageVm> Create(MessageVm message);
        public Task<List<MessageVm>> GetPrivateChat(MessageVm userDetails);
        public Task<List<ChatHistoryVM>> GetChatHistory(long FromId, long ToId);

        public Task<List<MessageVm>> GetUnReadMessages(long userId);
        public Task MarkRead(List<MessageVm> messages);

        public Task<int> GetAllUnReadMessagesCount(long userId); 
        public  Task<int> GetUnReadMessagesCountByUser(MessageVm messageVm);

    }
}
