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
    }
}
