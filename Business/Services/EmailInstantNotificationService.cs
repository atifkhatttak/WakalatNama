using Business.BusinessLogic;
using Business.Enums;
using Business.ViewModels;
using Data.Context;
using Data.DomainModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class EmailInstantNotificationService: BackgroundService
    {
        public ILogger<EmailInstantNotificationService> _logger { get; set; }
        public IServiceProvider _serviceProvider { get; }

        public EmailInstantNotificationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {

        }
        public async  Task ExecuteAsync<T>(T requestObject,int notificationMedium,int notificationType, CancellationToken stoppingToken)
        {
            await  ExecuteAsync(stoppingToken);
           
                string emailList = string.Empty;
            List<EmailLogs> emailLogs = new List<EmailLogs>();

                var subject = await GetNotificationSubject(notificationType);
                var body = await GetNotificationTemplate(notificationType);
                body = await PrepareNotificationBody(body, requestObject);
                emailList = await GetEmailList(requestObject);

            using (var scope = _serviceProvider.CreateScope())
            {
                var _emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                var _ctx = scope.ServiceProvider.GetRequiredService<WKNNAMADBCtx>();

                try
                {
                    _logger = scope.ServiceProvider.GetRequiredService<ILogger<EmailInstantNotificationService>>();
                    _logger.LogError($"Start: Instant Email{Environment.NewLine}");

                    var emailNotification = await _emailService.SendMailTrapEmail(subject, body, emailList);
                    _logger.LogError($"End: Instant Email{Environment.NewLine}");

                    var logs = await PreparePersistEmailLog<T>(requestObject, notificationType, notificationMedium, body, subject, true);

                    if (logs.Count() > 0)
                    {
                        await _ctx.EmailLogs.AddRangeAsync(logs);
                        await _ctx.SaveChangesAsync();

                    }
                }


                catch (Exception ex)
                {

                 var logs = await  PreparePersistEmailLog<T>(requestObject, notificationType, notificationMedium, body, subject, false);

                    if (logs.Count() > 0)
                    {
                        await _ctx.EmailLogs.AddRangeAsync(logs);
                        await _ctx.SaveChangesAsync();

                    }

                    _logger.LogError($"Instant Notification Error:{emailList}{Environment.NewLine} {ex.Message}");
                }
            }
            
        }

        public async Task<List<EmailLogs>> PreparePersistEmailLog<T>(T requestObject,int notificationType,int notificationMedium,string content,string subject,bool isSent)
        {
            List<EmailLogs> emailLogs = new List<EmailLogs>();

           if ( requestObject is FetchCaseDateNotificationDataVm)
            {
                FetchCaseDateNotificationDataVm fetchCaseDateVm = requestObject as FetchCaseDateNotificationDataVm;

                emailLogs.Add(new EmailLogs { Email = fetchCaseDateVm.CitizenEmail, UserId = fetchCaseDateVm.CitizenId, Content = content, Subject = subject, NotificationType = notificationType, SendMode = (int)SendMode.Instant,IsSent=isSent });
                emailLogs.Add(new EmailLogs { Email = fetchCaseDateVm.LawyerEmail, UserId = fetchCaseDateVm.LawyerId, Content = content, Subject = subject, NotificationType = notificationType, SendMode = (int)SendMode.Instant,IsSent= isSent });

            }

            return  await Task.FromResult(emailLogs);
        }

        private async Task<string> GetNotificationSubject(int notificationType)
        {
            string subject = string.Empty;
            switch (notificationType)
            {
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    subject = "Case Next Hearing Date Reminder";
                    break;
                default:
                    break;
            }
            return await Task.FromResult(subject);
        }

        private async Task<string> GetNotificationTemplate(int notificationType)
        {
            string notificationBody = string.Empty;
            switch (notificationType)
            {
                case 1:

                    break;
                case 2:
                    notificationBody = "Please reset your password by typing the OTP";
                    break;
                case 3:
                    notificationBody = "Your case: {CaseTitle} next hearing date is :{HearingDate}";
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    break;
                default:
                    notificationBody = "";
                    break;
            }
            return await Task.FromResult(notificationBody);
        }
        private async Task<string> PrepareNotificationBody<T>(string templateBody, T requestData)
        {
            var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var property in properties)
            {
                var value = property.GetValue(requestData);
                var propertyName = $@"{{{property.Name}}}";
                templateBody = templateBody.Replace(propertyName, value?.ToString());
            }

            return await Task.FromResult(templateBody);

        }

        private async Task<string> GetEmailList<T>(T requestObject) 
        {
            List<string> emailsList = new List<string>();
           
            if(requestObject is FetchCaseDateNotificationDataVm)
            {
                var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x=>x.Name== "LawyerEmail" || x.Name== "CitizenEmail");

               

                foreach (var property in properties)
                {
                    var value = property.GetValue(requestObject);

                    if (property.Name == "CitizenEmail" || property.Name== "LawyerEmail")
                        emailsList.Add(value?.ToString());

                }
            }

            return await Task.FromResult(string.Join(",",emailsList));
        }

        public void FillEmailLogs(List<EmailLogs> emails, FetchCaseDateNotificationDataVm user, bool isSent, string errorMessage)
        {
            var emailLog = new EmailLogs
            {
                Email = user.CitizenEmail,
                UserId = user.CitizenId,
                CaseId = user.CaseId,
                CaseDateId = user.CaseDateId,
                SendMode = (int)SendMode.Scheduled,
                Content = "",
                Subject = "",
                IsSent = isSent,
                ErrorMessage = errorMessage,
                NotificationType = (int)NotificationType.CaseDate
            };

            emails.Add(emailLog);
        }

        private void FillEmailLogObject<T>(T requestObject)
        {
            if(requestObject is FetchCaseDateNotificationDataVm)
            {
            //Fill EmailLogs
            }
        }
    }
}
