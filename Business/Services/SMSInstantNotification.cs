using Business.Enums;
using Business.ViewModels;
using Data.Context;
using Data.DomainModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class SMSInstantNotification : BackgroundService
    {
        public IServiceProvider _serviceProvider { get; }
        private  ILogger<SMSInstantNotification> _logger;
        public SMSInstantNotification(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
          
        }
        public async Task ExecuteAsync<T>(T requestObject, int notificationMedium, int notificationType, CancellationToken stoppingToken)
        {
            await ExecuteAsync(stoppingToken);

            string emailList = string.Empty;
            List<EmailLogs> emailLogs = new List<EmailLogs>();

            var body = await GetNotificationTemplate(notificationType);
            body = await PrepareNotificationBody(body, requestObject);
           var contactNumberList = await GetContactNumberList(requestObject);

            using (var scope = _serviceProvider.CreateScope())
            {
                var _smsService = scope.ServiceProvider.GetRequiredService<ISMSService>();
                var _ctx = scope.ServiceProvider.GetRequiredService<WKNNAMADBCtx>();

                try
                {
                    _logger = scope.ServiceProvider.GetRequiredService<ILogger<SMSInstantNotification>>();
                    _logger.LogError($"Start: Instant Email{Environment.NewLine}");

                    var emailNotification = await _smsService.SendSMSAsync( string.Join(",",contactNumberList), body);
                    _logger.LogError($"End: Instant Email{Environment.NewLine}");
                    var logs = await PreparePersistSmsLog<T>(requestObject, notificationType, body, true);

                    if (logs.Count() > 0)
                    {
                        await _ctx.SMSLogs.AddRangeAsync(logs);
                        await _ctx.SaveChangesAsync();

                    }
                }
                catch (Exception ex)
                {
                    var logs = await PreparePersistSmsLog<T>(requestObject, notificationType, body, false);

                    if (logs.Count() > 0)
                    {
                        await _ctx.SMSLogs.AddRangeAsync(logs);
                        await _ctx.SaveChangesAsync();

                    }

                    _logger.LogError($"Instant Notification Error:{emailList}{Environment.NewLine} {ex.Message}");
                }
            }
        }

        public async Task<List<SMSLogs>> PreparePersistSmsLog<T>(T requestObject, int notificationType, string content, bool isSent)
        {
            List<SMSLogs> emailLogs = new List<SMSLogs>(); 

            if (requestObject is FetchCaseDateNotificationDataVm)
            {
                FetchCaseDateNotificationDataVm fetchCaseDateVm = requestObject as FetchCaseDateNotificationDataVm;

                emailLogs.Add(new SMSLogs { PhoneNumber = fetchCaseDateVm.CitizenContactNumber, UserId = fetchCaseDateVm.CitizenId, Content = content,  NotificationType = notificationType, SendMode = (int)SendMode.Instant, IsSent = isSent });
                emailLogs.Add(new SMSLogs { PhoneNumber = fetchCaseDateVm.LawyerContactNumber, UserId = fetchCaseDateVm.LawyerId, Content = content, NotificationType = notificationType, SendMode = (int)SendMode.Instant, IsSent = isSent });

            }

            return await Task.FromResult(emailLogs);
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

        private async Task<string> GetContactNumberList<T>(T requestObject)
        {
            List<string> emailsList = new List<string>();

            if (requestObject is FetchCaseDateNotificationDataVm)
            {
                var FetchCaseData = requestObject as FetchCaseDateNotificationDataVm;

                emailsList.Add(FetchCaseData?.CitizenContactNumber);
                emailsList.Add(FetchCaseData?.LawyerContactNumber);
            }

            return await Task.FromResult(string.Join(",", emailsList));
        }

        public void FillSmsLogs(List<SMSLogs> smsLogs, FetchCaseDateNotificationDataVm user, bool isSent, string errorMessage)
        {
            var smsLog = new SMSLogs 
            {
                PhoneNumber = user.CitizenContactNumber,
                UserId = user.CitizenId,
                CaseId = user.CaseId,
                CaseDateId = user.CaseDateId,
                SendMode = (int)SendMode.Scheduled,
                Content = "",
                IsSent = isSent,
                ErrorMessage = errorMessage,
                NotificationType = (int)NotificationType.CaseDate
            };

            smsLogs.Add(smsLog);
        }

        private void FillEmailLogObject<T>(T requestObject)
        {
            if (requestObject is FetchCaseDateNotificationDataVm)
            {
                //Fill EmailLogs
            }
        }

    }
}
