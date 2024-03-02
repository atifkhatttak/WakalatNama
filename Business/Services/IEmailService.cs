using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Business.Services
{
   public interface IEmailService
    {
        public Task<bool> SendMailTrapEmail(string subject, string body, string to);
    }
}
