using System;
using System.Reflection;
using ChatShared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;

namespace ChatClient.Services
{
    public class StateContainer : IAsyncDisposable
    {

        private NavigationManager _navigationManager;
        public HubConnection? hubConnection;
        public Dictionary<string,User> userList = new Dictionary<string, User>();
        public User me = new User { };
        public User ToUser = new User { };
        public string userName { get; set; } = String.Empty;
        public string token { get; set; } = String.Empty;



        public StateContainer(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }

        public event EventHandler userConnectionEvent = default!;
        public event EventHandler OnMessageReceiveEvent = default!;

        public async Task OnConnectAsync()
        {
            if (hubConnection is not null && hubConnection.State == HubConnectionState.Connected)
            {
                return;
            }

            hubConnection = new HubConnectionBuilder()
                .WithUrl(_navigationManager.ToAbsoluteUri("/chathub"), options =>
                {
                    
                        options.AccessTokenProvider = () => Task.FromResult(token);
                })
                .Build();

            hubConnection.On<Dictionary<string,User>>("OnUserUpdate", UserList =>
            {
                userList = UserList;
                userConnectionEvent?.Invoke(this,EventArgs.Empty);
            });

            hubConnection.On<string,string>("ReceiveMessage", (username,message) =>
            {
                if (!me.messages.ContainsKey(username))
                {
                    me.messages.Add(username, new List<string>());
                }
                me.messages[username].Add(message);
                OnMessageReceiveEvent?.Invoke(this, EventArgs.Empty);
            });


            await hubConnection.StartAsync();

            if (hubConnection is not null)
            {
                me = new User
                {
                    UserName = userName,
                    ConnectionId = hubConnection.ConnectionId!
                };

                await hubConnection.SendAsync("UpdateUserList", me);
            }
        }

        public async Task SendMessage(User sendTo, string message)
        {
            if (hubConnection is not null)
            {
                await hubConnection.SendAsync("SendMessage", sendTo, me, message);
                if (!me.messages.ContainsKey(sendTo.UserName))
                {
                    me.messages.Add(sendTo.UserName, new List<string>());
                }
                me.messages[sendTo.UserName].Add($"{me.UserName} : {message}");
                OnMessageReceiveEvent?.Invoke(this, EventArgs.Empty);
            }
        }

        public void SetToUser(User user)
        {
            ToUser = user;
            OnMessageReceiveEvent?.Invoke(this, EventArgs.Empty);
        }

        public async ValueTask DisposeAsync()
        {
            if (hubConnection is not null)
            {
                await hubConnection.SendAsync("RemoveFromUserList", me);
                await hubConnection.DisposeAsync();
            }
        }

        
    }
}

