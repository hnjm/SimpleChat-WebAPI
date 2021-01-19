using System;

namespace SimpleChat.ViewModel.SignalR
{
    public record OnJoinToGroup
    {
        public string ConnectionId { get; set; }
    }
}
