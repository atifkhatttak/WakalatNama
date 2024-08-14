using Business.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public interface ISMSService
    {
        public Task<CorpSMSVm> SendSMSAsync(string toNumber, string text);
        public Task ConnectSMSAPI();

    }
}
