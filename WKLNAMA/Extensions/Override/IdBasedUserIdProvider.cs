using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace WKLNAMA.Extensions.Override
{
    public class IdBasedUserIdProvider  : IUserIdProvider
    {
        public virtual string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst("UserId")?.Value!;
        }
    }
}
