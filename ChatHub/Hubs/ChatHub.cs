using System;
using Microsoft.AspNetCore.SignalR;
using ChatShared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;

namespace ChatHub.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChatHub : Hub
    {
        private Dictionary<string, User> _users;
        private IUserIdProvider _userIdProvider;
        public ChatHub(Dictionary<string, User> users, IUserIdProvider userIdProvider)
        {
            _users = users;
            _userIdProvider = userIdProvider;
        }

        public async Task SendMessage(User user, User sender, string message)
        {
            await Clients.User(user.UserName).SendAsync("ReceiveMessage",sender.UserName,$"{sender.UserName} : {message}");
            
            //await Clients.All.SendAsync("ReceiveMessage", $"{sender.UserName} : {message}");
        }

        public async Task UpdateUserList(User user)
        {
            if (_users.ContainsKey(user.ConnectionId))
            {
                return;
            }

            user.UserName = Context.UserIdentifier!;
            _users.Add(user.ConnectionId, user);
            await Clients.All.SendAsync("OnUserUpdate", _users);
        }

        public async Task RemoveFromUserList(string connectionId)
        {
            if (!_users.ContainsKey(connectionId))
            {
                return;
            }
            _users.Remove(connectionId);
            await Clients.All.SendAsync("OnUserUpdate", _users);

        }

            public override async Task OnConnectedAsync()
            {
            
            await base.OnConnectedAsync();
            }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await RemoveFromUserList(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}

