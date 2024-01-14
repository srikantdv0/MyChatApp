using System;
using Microsoft.AspNetCore.SignalR;

namespace ChatHub.Hubs
{
    public class NameUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
                return connection.User?.Identity?.Name;   
        }
    }
}

