using System;
using ChatClient.Services;
using ChatShared.Models;
using Microsoft.AspNetCore.Components;

namespace ChatClient.Components
{
    public partial class ChatWindow
    {
       

        [Inject]
        StateContainer stateContainer { get; set; } = default!;

        private ChatForm chatForm = new ChatForm();

        private async Task Submit()
        {
            await stateContainer.SendMessage(stateContainer.ToUser, chatForm.Message);
            chatForm.Message = string.Empty;
        }
    }
}

