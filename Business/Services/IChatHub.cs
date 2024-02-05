using Business.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public interface IChatHub
    {
       // public Task DirectMessage(string message);
        public Task DirectMessage(MessageVm model);
        public Task DeleteMessage(string message, string toWho); 

    }
}
