using Business.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using System.Xml.Serialization;

namespace Business.BusinessLogic
{
    public class SmsService : ISMSService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmsService> _logger;
        private readonly string _apiBaseUrl;
        private readonly string _authUrl;
        private readonly string _sendMessageUrl; 
        private readonly string _fromNumber;
        private readonly string _password;
        private string _sessionId;

        public string ApiBaseUrl => _apiBaseUrl;

        public string AuthUrl => _authUrl;

        public string SendMessageUrl => _sendMessageUrl;

        public string FromNumber => _fromNumber;

        public string Password => _password;

        public string SessionId => _sessionId;

        public SmsService(IConfiguration configuration,ILogger<SmsService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _apiBaseUrl = "https://telenorcsms.com.pk:27677/corporate_sms2/api";
            _authUrl = "/auth.jsp";
            _sendMessageUrl = "/sendsms.jsp";
            _password = "L9$eQf7@bKr2!zD4mXn3";
            _fromNumber = "923400094776";
        }
        /// <summary>
        /// The method will be used to optain session id of sms API
        /// </summary>
        /// <returns></returns>
        public async Task  ConnectSMSAPI()  
        {
            var httpClient = new HttpClient();

            var response = await httpClient.GetAsync($"{_apiBaseUrl}{AuthUrl}?msisdn={FromNumber}&password={Password}");
          
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();

                CorpSMSVm corpSMSVm = new CorpSMSVm();
                XmlSerializer serializer = new XmlSerializer(typeof(CorpSMSVm));
                using (StringReader reader = new StringReader(responseBody))
                {
                    corpSMSVm =  (CorpSMSVm)(serializer.Deserialize(reader));
                }

                if (corpSMSVm.Response == "OK")
                    _sessionId = corpSMSVm.Data;
                else
                    _sessionId = "";

                _logger.LogInformation($"Connected: SessionId:{SessionId}");
            }

        }
        /// <summary>
        /// The method will be used to send sms to end user, 
        /// </summary>
        /// <param name="toNumber">To Number</param>
        /// <param name="text">Text</param>
        /// <returns></returns>
        public async Task<CorpSMSVm> SendSMSAsync(string toNumber,string text)
        {
            CorpSMSVm corpSMSVm = new CorpSMSVm();

            string messageId = string.Empty;
            var httpClient = new HttpClient();

            var content = new StringContent("", Encoding.UTF8, "application/json");

            string url = $"{_apiBaseUrl}{SendMessageUrl}?session_id={SessionId}&text={text}&to={toNumber}";
             
            var response = await httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();

                XmlSerializer serializer = new XmlSerializer(typeof(CorpSMSVm));
                using (StringReader reader = new StringReader(responseBody))
                {
                    corpSMSVm = (CorpSMSVm)(serializer.Deserialize(reader));
                }

                if (corpSMSVm.Response == "OK")
                    messageId = corpSMSVm.Data;
                else
                    messageId = corpSMSVm.Data;

                _logger.LogInformation($"Message Sent: Messages Id:{messageId}");
            }

            return corpSMSVm;

        }


    }

    [XmlRoot("corpsms")] 

    public class CorpSMSVm
    {
        [XmlElement("command")]
        public string Command { get; set; }

        [XmlElement("data")]
        public string Data { get; set; }

        [XmlElement("response")]
        public string Response { get; set; }


    }
}
