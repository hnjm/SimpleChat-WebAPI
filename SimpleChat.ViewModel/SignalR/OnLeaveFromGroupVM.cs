using System;

namespace SimpleChat.ViewModel.SignalR
{
    public record OnLeaveFromGroupVM
    {
        public Guid GroupId { get; set; }
        public Guid UserId { get; set; }
    }
}
