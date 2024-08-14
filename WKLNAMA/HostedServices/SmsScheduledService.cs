using Business.Chat_Hub;
using Business.Services;
using Microsoft.AspNetCore.SignalR;

namespace WKLNAMA.HostedServices
{
    public class SmsScheduledService: BackgroundService
    {
        private readonly ILogger<ServerNotificationService> _logger;
        private static readonly TimeSpan Period = TimeSpan.FromSeconds(5);
        private TimeSpan _scheduleTime { set; get; }
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider; 

        public SmsScheduledService(ILogger<ServerNotificationService> logger, IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _configuration = configuration;
            this._serviceProvider = serviceProvider;
            _scheduleTime = TimeSpan.FromHours(Convert.ToInt16(_configuration["HostedService:SmsSchedledTime"]));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(_scheduleTime);

            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync())
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var _caseService = scope.ServiceProvider.GetRequiredService<ICasesRepository>();

                    var _notificationService = scope.ServiceProvider.GetRequiredService<INotificationRepository>();

                    var reminderNotificationData = await _caseService.GetCaseNextDateReminderDataForSms();

                    if (reminderNotificationData?.Count() > 0)
                    {
                        await _notificationService.SendSmsSchedledNotification(reminderNotificationData, stoppingToken);
                    }
                }
            }
        }
    }
}
