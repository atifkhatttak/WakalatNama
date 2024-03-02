using Business.Services;
using Data.Context;
using Data.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.BusinessLogic
{
    public class NotificationRepository: BaseRepository<Notification>, INotificationRepository
    {
        public WKNNAMADBCtx _ctx { get; }

        public NotificationRepository(WKNNAMADBCtx ctx): base(ctx) 
        {
            _ctx = ctx;
        }

    }
}
