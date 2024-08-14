using Business.Chat_Hub;
using Business.Enums;
using Business.Helpers;
using Business.Services;
using Business.ViewModels;
using Data.Context;
using Data.DomainModels;
using FirebaseAdmin.Auth;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3.Data;
using Google.Apis.FirebaseCloudMessaging.v1;
using Google.Apis.FirebaseCloudMessaging.v1.Data;
using Google.Apis.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Business.BusinessLogic
{
    public class NotificationRepository : BaseRepository<Data.DomainModels.Notification>, INotificationRepository
    {
        private readonly ChatHub _chatHUb;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NotificationRepository> _logger;

        public WKNNAMADBCtx _ctx { get; }

        public NotificationRepository(WKNNAMADBCtx ctx, ILogger<CasesRepository> logger, ChatHub chatHUb, IServiceProvider serviceProvider,ILogger<NotificationRepository> logger1) : base(ctx)
        {
            _ctx = ctx;
            _chatHUb = chatHUb;
            _serviceProvider = serviceProvider;
            _logger = logger1;
         }

        public async Task<List<NotificationVm>> GetAllUnReadNotification(long UserId)
        {
            return await (from n in _ctx.Notifications
                          join up in _ctx.UserProfiles on n.FromUserId equals up.UserId
                          where n.ToUserId == UserId && !n.IsRead && !(n.IsDeleted && up.IsDeleted)
                          select new NotificationVm
                          {
                              ImageUrl = up.ProfilePicUrl,
                              Content = n.Content,
                              Id = n.Id,
                              IsRead = n.IsRead,
                              FromUserId = n.FromUserId,
                              ToUserId = n.ToUserId,
                              NotificationType = n.NotificationType,
                              CreatedDate = n.CreatedDate
                          })
                 .OrderByDescending(n => n.Id)
                 .ToListAsync();
        }

        public async Task<List<Data.DomainModels.Notification>> MarkIsRead(List<long> notificationIds)
        {
            List<Data.DomainModels.Notification> _notification = await _ctx.Notifications.Where(x => notificationIds.Contains(x.Id)).ToListAsync();

            if (_notification?.Count() > 0)
                _notification.ForEach(x => x.IsRead = true);

            await _ctx.SaveChangesAsync();

            return _notification!;
        }

        public async Task<List<NotificationVm>> GetAllNotificationByUser(long UserId)
        {
            return await
                (from n in _ctx.Notifications
                 join up in _ctx.UserProfiles on n.FromUserId equals up.UserId
                 where n.ToUserId == UserId && !(n.IsDeleted && up.IsDeleted)
                 select new NotificationVm
                 {
                     ImageUrl = up.ProfilePicUrl,
                     Content = n.Content,
                     Id = n.Id,
                     IsRead = n.IsRead,
                     FromUserId = n.FromUserId,
                     ToUserId = n.ToUserId,
                     NotificationType = n.NotificationType,
                     CreatedDate = n.CreatedDate
                 })
                 .OrderByDescending(n => n.Id)
                 .ToListAsync();
        }

        public async Task<Data.DomainModels.Notification> AddNotification(NotificationVm model)
        {
            Data.DomainModels.Notification notificationModel = new Data.DomainModels.Notification
            {
                Content = model.Content,
                FromUserId = model.FromUserId,
                ToUserId = model.ToUserId,
                NotificationType = model.NotificationType,
            };
            _ctx.Notifications.Add(notificationModel);

            await _ctx.SaveChangesAsync();

            return notificationModel;
        }

        public async Task BroadCastAndSaveNotification(NotificationVm model)
        {
            var notification = await AddNotification(model);

            if (notification != null)
            {
                model.Id = notification.Id;
                var count = await GetAllUnReadNotificationCount(model.ToUserId);
                //await _chatHUb.SendNotificationToClient(model, count, model.ToUserId);
               await SendPushNotification(model.ToUserId,"New Notification",model.Content);
            }
        }

        public async Task<int> GetAllUnReadNotificationCount(long UserId)
        {
            return await _ctx.Notifications.Where(x => x.ToUserId == UserId && x.IsRead != true).CountAsync();
        }

        public async Task<bool> MarkAllNotificationRead(long UserId)
        {
            return UserId > 0
                &&
                 await _ctx.Notifications.Where(x => x.ToUserId == UserId && !x.IsRead && !x.IsDeleted)
                 .ExecuteUpdateAsync(x => x.SetProperty(z => z.IsRead, true)) > 0;
        }

        public async Task<bool> MarkSingleNotification(long UserId, long NotificationId)
        {
            return UserId > 0 && NotificationId>0
                 &&
                  await _ctx.Notifications.Where(x => x.Id==NotificationId && x.ToUserId == UserId && !x.IsRead && !x.IsDeleted)
                  .ExecuteUpdateAsync(x => x.SetProperty(z => z.IsRead, true)) > 0;
        }

        public async Task SendEmailScheduledNotification(List<FetchCaseDateNotificationDataVm> userData,CancellationToken cancellationToken) 
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                 
                List<EmailLogs> emails = new List<EmailLogs>();



                #region Emails
                await Parallel.ForEachAsync( userData, new ParallelOptions
               { MaxDegreeOfParallelism = 10,CancellationToken= cancellationToken }, async (user, cancellationToken) =>
                {
                    string subject = "Next Case Hearing Date";
                    string body = $"Your case is scheduleded at {user.HearingDate.ToString("yyyy/MM/dd hh:mm tt")}";

                    #region Citizen Email
                    try
                    {

                   
                    var citizenEmailRequest = await _emailService.SendMailTrapEmail(subject, body, user.CitizenEmail);
                        FillEmailLogs(emails,user,true,string.Empty,true,subject,body);
                    }
                    catch(Exception ex)
                    {
                        string errorMessage = $@"Scheduled Email Notification {Environment.NewLine} Citizen-Email: {user.CitizenEmail} Failed {Environment.NewLine} Error :{ex.Message}";
                        FillEmailLogs(emails,user,false,string.Empty,true,subject,body);

                        _logger.LogError(errorMessage);
                    }

                    #endregion

                    #region Lawyer Email
                    try
                    {
                        var lawyerEmailRequest = await _emailService.SendMailTrapEmail(subject, body, user.LawyerEmail);
                        FillEmailLogs(emails, user, true, string.Empty,false,subject,body);

                    }
                    catch (Exception ex)
                    {
                        string errorMessage = $@"Scheduled Email Notification {Environment.NewLine} Lawyer-Email : {user.LawyerEmail} Failed {Environment.NewLine} Error :{ex.Message}";
                        FillEmailLogs(emails,user,false, errorMessage,false,subject,body);

                        _logger.LogError(errorMessage);
                    }

                    #endregion

                    

                });

                if ( emails.Count() > 0 )
                {
                   await _ctx.EmailLogs.AddRangeAsync(emails);
                  await  _ctx.SaveChangesAsync();
                }

                #endregion

            }

            


            //SMS
        }

        public async Task SendSmsSchedledNotification(List<FetchCaseDateNotificationDataVm> userData, CancellationToken cancellationToken)
        { 
            using (var scope = _serviceProvider.CreateScope())
            {
                var _smsService = scope.ServiceProvider.GetRequiredService<ISMSService>();

                List<SMSLogs> smsLogs = new List<SMSLogs>();

                await _smsService.ConnectSMSAPI();

                #region Emails
                await Parallel.ForEachAsync(userData, new ParallelOptions
                { MaxDegreeOfParallelism = 10, CancellationToken = cancellationToken }, async (user, cancellationToken) =>
                {
                    string subject = "Next Case Hearing Date";
                    string body = $"Your case is scheduleded at {user.HearingDate.ToString("yyyy/MM/dd hh:mm tt")}";


                    #region Citizen SMS
                    try
                    {


                        var citizenSMSRequest = await _smsService.SendSMSAsync(user.CitizenContactNumber, body);

                        string errorMessage = string.Empty;
                        
                        if (citizenSMSRequest.Response.ToLower() == "ok")
                            errorMessage = citizenSMSRequest.Response;

                        FillSMSLogs(smsLogs, user, true, errorMessage, body, true);
                    }
                    catch (Exception ex)
                    {
                        string errorMessage = $@"Scheduled SMS Notification {Environment.NewLine} Citizen-Contact Number: {user.CitizenEmail} Failed {Environment.NewLine} Error :{ex.Message}";
                        FillSMSLogs(smsLogs, user, false, errorMessage, body, true);

                        _logger.LogError(errorMessage);
                    }

                    #endregion

                    #region Lawyer SMS
                    try
                    {
                       

                        var lawyerSMSRequest = await _smsService.SendSMSAsync(user.LawyerContactNumber, body);
                        
                        string errorMessage = string.Empty;

                        if (lawyerSMSRequest.Response.ToLower() == "ok")
                            errorMessage = lawyerSMSRequest.Response;

                        FillSMSLogs(smsLogs, user, true, errorMessage, body, false);

                    }
                    catch (Exception ex)
                    {
                        string errorMessage = $@"Scheduled SMS Notification {Environment.NewLine} Lawyer-SMS : {user.LawyerEmail} Failed {Environment.NewLine} Error :{ex.Message}";
                        FillSMSLogs(smsLogs, user, false, errorMessage, body, false);

                        _logger.LogError(errorMessage);
                    }

                    #endregion

                });

                if (smsLogs.Count() > 0)
                {
                    await _ctx.SMSLogs.AddRangeAsync(smsLogs);
                    await _ctx.SaveChangesAsync();
                }

                #endregion

            }




            //SMS
        }


        public void FillEmailLogs(List<EmailLogs> emails, FetchCaseDateNotificationDataVm user,bool isSent,string errorMessage,bool isCitizen,string subject,string content) 
        { 
            var emailLog = new EmailLogs 
                       { 
                           Email = isCitizen?  user.CitizenEmail:user.LawyerEmail,
                           UserId = isCitizen ? user.CitizenId: user.LawyerId, 
                           CaseId = user.CaseId, 
                           CaseDateId = user.CaseDateId, 
                           SendMode=(int)SendMode.Scheduled,
                           Content=subject,
                           Subject=content,
                           IsSent =isSent,
                           ErrorMessage=errorMessage,
                           NotificationType=(int)NotificationType.CaseDate
                       };

            emails.Add(emailLog);
        }

        public void FillSMSLogs(List<SMSLogs> sms, FetchCaseDateNotificationDataVm user, bool isSent, string errorMessage,string content,bool isCitizen)
        { 
            var smsLog = new SMSLogs
            {
                PhoneNumber = isCitizen ? user.CitizenContactNumber: user.LawyerContactNumber,
                UserId = isCitizen ? user.CitizenId: user.LawyerId,
                CaseId = user.CaseId,
                CaseDateId = user.CaseDateId,
                SendMode = (int)SendMode.Scheduled,
                IsSent =string.IsNullOrEmpty(errorMessage)? true:false,
                ErrorMessage = errorMessage,
                Content=content,
                NotificationType = (int)NotificationType.CaseDate
            };

            sms.Add(smsLog);
        }

        public async Task<string> GenerateTokenAsync()
        {
            var json = System.IO.File.ReadAllText("path/to/serviceAccountKey.json");
            var credentials = GoogleCredential.FromJson(json);

            FirebaseApp.Create(new AppOptions
            {
                Credential = credentials,
                ServiceAccountId = "your-service-account-id",
            });

            var auth = FirebaseAuth.DefaultInstance;
            //var token = await auth.CreateCustomTokenAsync();

            return string.Empty;// token.AccessToken;
        }



        /// <summary>
        /// Firebase push notifcation
        /// </summary>
        /// <param name="userId">Provider userid to who you want to send notification</param>
        /// <param name="title">Notification title</param>
        /// <param name="body">Notification body</param>
        /// <returns></returns>
        public async Task<bool> SendPushNotification(long userId,string title,string body)
        {
            bool response = false;
            //token = "fuODiYOoS_SMQJLD-dTRaN:APA91bGTQFKPlzuhPFgbgHAAOomKIzvEKRxX1TlVyVLkNCq5xKGQkFP-AMXSZf9iaqlHdLHC-Hai8rvNivKfbY_w_7IyVzJKDAFsfafeWeYi9PuyXu28r7PwBlJrKHgo0qgF9SUr5J4x";
            try
            {
                var toUser = _ctx.UserProfiles.FirstOrDefault(x => x.UserId == userId && x.IsActive == true && x.IsPushAlert == true && !x.IsDeleted && !string.IsNullOrEmpty(x.DeviceToken));
                if (toUser == null) return response;


                // Path to the service account key file
                GoogleCredential credential;
                using (var stream = new FileStream("wakalatnaama-f6493-12d4dc3dc395.json", FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleCredential.FromStream(stream)
                        .CreateScoped("https://www.googleapis.com/auth/cloud-platform");
                }

                _logger.LogInformation("Accessing Google auth token");
                var googleCredential = (GoogleCredential)credential;
                var credentials = (ICredential)credential.UnderlyingCredential;
                var accessToken = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
                _logger.LogInformation("Google auth token: "+accessToken);


                var data = new
                {
                    message = new
                    {
                        token =toUser?.DeviceToken,
                        notification = new
                        {
                            title = title,
                            body = body
                        },
                    }
                };

                var json = JsonConvert.SerializeObject(data);

                var byteArray = Encoding.UTF8.GetBytes(json);
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/v1/projects/wakalatnaama-f6493/messages:send");
                tRequest.Method = "POST";
                tRequest.ContentType = "application/json";
                tRequest.Headers.Add("Authorization", $"Bearer {accessToken}");
                //tRequest.Headers.Add(string.Format("Sender: id={0}", "528140156856"));
                tRequest.ContentLength = byteArray.Length;

                using (Stream dataStream =await tRequest.GetRequestStreamAsync())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }

                try
                {
                    using (WebResponse tResponse =await tRequest.GetResponseAsync())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        using (StreamReader tReader = new StreamReader(dataStreamResponse))
                        {
                            string sResponseFromServer =await tReader.ReadToEndAsync();
                            _logger.LogInformation($"Firebase: {sResponseFromServer}");
                        }
                        response = true;
                    }

                    return response;
                }
                catch (WebException webEx)
                {
                    using (var responseStream = webEx.Response.GetResponseStream())
                    using (var reader = new StreamReader(responseStream))
                    {
                        string responseText = reader.ReadToEnd();
                        _logger.LogError("Firebase: WebException: " + webEx.Message);
                        _logger.LogError("Firebase: from server: " + responseText);
                    }
                    return response;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Firebase Exception: " + ex.Message);
                throw;
            }
        }

        public async Task<T> SendInstantNotification<T>(T requestObject, int notificationMedium, int notificationType, CancellationToken stoppingToken)
        {
           var notificationTemplate = await  GetNotificationTemplate(notificationType);
            var notificationSUbject = await GetNotificationSubject(notificationType);
           var notificationFinalBody = await PrepareNotificationBody(notificationTemplate, requestObject);

            using (var scope = _serviceProvider.CreateScope())
            {
                var _emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                if (notificationMedium == (int)NotificationMedium.Email)
                {
                    var emailNotification  =   await _emailService.SendMailTrapEmail(notificationSUbject, notificationFinalBody, requestObject.GetType().GetProperty("Email").GetValue(requestObject)?.ToString());
                }
                else if (notificationMedium == (int)NotificationMedium.SMS)
                {

                }
                else if(notificationMedium==(int)NotificationMedium.Both)
                { }
            }

                return await Task.FromResult<T>(requestObject);
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
    }
}
