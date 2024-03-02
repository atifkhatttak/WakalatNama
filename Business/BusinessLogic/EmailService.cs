using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Business.Services;

namespace Business.BusinessLogic
{
    public class EmailService : IEmailService
    {
        public  IConfiguration _configuration { set; get; }
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public bool SendEmailWithoutTemplate(string to, string subject, string body, bool isBodyHtml = false)
        {
            var smtp = new SmtpClient
            {
                Host = _configuration["EmailConfiguration:Host"]!,
                Port = Convert.ToInt32(_configuration["EmailConfiguration:Port"]!),
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_configuration["EmailConfiguration:Email"]!, _configuration["EmailConfiguration:Password"]!),
                Timeout = 10000
            };
            using (var message = new MailMessage(_configuration["EmailConfiguration:Email"]!, to)
            {
                IsBodyHtml = isBodyHtml,
                Subject = subject,
                Body = body
            })
            {
                try
                {

                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    smtp.Send(message);
                    return true;
                }
                catch (SmtpException e)
                {
                    string er = e.Message;
                }
            }

            return false;
        }


        public bool SendEmailWitAttachment(string to, string subject, string body, byte[] content, bool isBodyHtml = false, string DocName = "Certificate.pdf")
        {
            var smtp = new SmtpClient
            {
                Host = _configuration["EmailConfiguration:Host"]!,
                Port = Convert.ToInt32(_configuration["EmailConfiguration:Port"]!),
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_configuration["EmailConfiguration:Email"]!, _configuration["EmailConfiguration:Password"]!),
                Timeout = 10000
            };
            using (var message = new MailMessage(_configuration["EmailConfiguration:Email"]!, to)
            {
                IsBodyHtml = isBodyHtml,
                Subject = subject,
                Body = body,

            })
            {
                try
                {
                    using (MemoryStream ms = new MemoryStream(content))
                    {
                        message.Attachments.Add(new Attachment(ms, DocName));
                        smtp.Send(message);
                    }

                    return true;
                }
                catch (SmtpException e)
                {
                    string er = e.Message;
                }
            }

            return false;
        }


        public string ReplaceEmailTokenWithValues(List<KeyValuePair<string, string>> TokenWithValues, string EmailContents)
        {
            string _values = string.Empty;
            foreach (var tokenWithValue in TokenWithValues)
            {
                _values = string.IsNullOrEmpty(tokenWithValue.Value) ? "" : tokenWithValue.Value;
                EmailContents = EmailContents.Replace(tokenWithValue.Key, _values);
            }

            return EmailContents;
        }

        public string ReplaceEmailTemplateWithTokens(string _TemplatePath, List<KeyValuePair<string, string>> _Tokens)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(_TemplatePath))
            {
                body = reader.ReadToEnd();
            }

            foreach (var _Token in _Tokens)
            {
                body = body.Replace(_Token.Key, _Token.Value);
            }

            return body;
        }

        public async Task<bool> SendEmailWithoutTemplateMailGun(string to, string subject, string body, bool isBodyHtml = false)
        {
            using (var client = new HttpClient
            {
                BaseAddress = new Uri(_configuration["MailGunEmailConfiguration:ApiBaseUri"]!)
            })
            {
                client.DefaultRequestHeaders.Authorization =
                  new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes(_configuration["MailGunEmailConfiguration:Api"]!)));

                var content = new FormUrlEncodedContent(new[]
                {
                      new KeyValuePair<string, string>("from", _configuration["MailGunEmailConfiguration:Email"]!),
                      new KeyValuePair<string, string>("to", to),
                      new KeyValuePair<string, string>("subject", subject),
                      new KeyValuePair<string, string>("html", body)
                });

                try
                {
                    var res = await client.PostAsync(_configuration["MailGunEmailConfiguration:RequestUri"]!, content).ConfigureAwait(false);
                    if (res.StatusCode == HttpStatusCode.OK)
                        return true;
                    else
                        return false;

                }
                catch (Exception ex)
                {
                    return false;
                }
            }

        }


        public async Task<bool> SendEmailWitAttachmentMailGun(string to, string subject, string body, byte[] content, bool isBodyHtml = false, string DocName = "Certificate.pdf")
        {
            using (var client = new HttpClient
            {
                BaseAddress = new Uri(_configuration["MailGunEmailConfiguration:ApiBaseUri"]!)
            })
            {
                client.DefaultRequestHeaders.Authorization =
                  new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes(_configuration["MailGunEmailConfiguration:Api"]!)));

                //var Emailcontent = new MultipartFormDataContent(new[]
                //{
                //      new KeyValuePair<string, string>("from", MailGunEmailConfiguration.Email),
                //      new KeyValuePair<string, string>("to", to),
                //      new KeyValuePair<string, string>("subject", subject),
                //      new KeyValuePair<string, string>("html", body)
                //});


                var Emailcontent = new MultipartFormDataContent();
                Emailcontent.Add(new StringContent(subject), "subject");
                Emailcontent.Add(new StringContent(_configuration["MailGunEmailConfiguration:Email"]!), "from");
                Emailcontent.Add(new StringContent(to), "to");
                Emailcontent.Add(new StringContent(body), "html");
                ByteArrayContent fileContent = new ByteArrayContent(content);

                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "attachment",
                    FileName = DocName
                };

                Emailcontent.Add(fileContent);


                try
                {
                    var res = await client.PostAsync(_configuration["MailGunEmailConfiguration:RequestUri"]!, Emailcontent).ConfigureAwait(false);
                    if (res.StatusCode == HttpStatusCode.OK)
                        return true;
                    else
                        return false;

                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public async Task<bool> SendMailTrapEmail(string subject,string body,string to)
        {
            var client = new SmtpClient(_configuration["MailTrapEmail:Host"]!, Convert.ToInt16(_configuration["MailTrapEmail:Port"]!))
            {
                Credentials = new NetworkCredential(_configuration["MailTrapEmail:UserName"]!, _configuration["MailTrapEmail:API"]!),
                EnableSsl = true
            }; 
            await client.SendMailAsync(_configuration["MailTrapEmail:Email"]!, to, subject, body);
            return true;
        }

    }
}
