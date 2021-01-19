using System;

namespace SimpleChat.ViewModel.SignalR
{
    public record OnLeaveFromGroup
    {
        public string ConnectionId { get; set; }
    }
}
