
using Business.Services;
using Microsoft.AspNetCore.SignalR;
using WKLNAMA.AppHub;

namespace WKLNAMA.HostedServices
{
    public class ServerNotificationService : BackgroundService
    {
        private readonly ILogger<ServerNotificationService> _logger;
        private readonly IHubContext<ChatHub, IChatHub> _context;
        private static readonly TimeSpan Period=TimeSpan.FromSeconds(5);
        private TimeSpan _scheduleTime { set; get; }
        private readonly IConfiguration _configuration;

        /// <summary>
        /// The backround service will execute after every two hour
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        public ServerNotificationService(IHubContext<ChatHub, IChatHub> context, ILogger<ServerNotificationService> logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
            _scheduleTime = TimeSpan.FromHours(Convert.ToInt16(_configuration["HostedService:ScheduledTime"]));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(_scheduleTime);  
            
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync())
            {
              //  _logger.LogInformation("Server is logging {Service} {Time}", nameof(ServerNotificationService),DateTime.Now);
               await _context.Clients.All.UnReadMessage(new List<Business.ViewModels.MessageVm>(), 5);
            }
         }
    }
}
