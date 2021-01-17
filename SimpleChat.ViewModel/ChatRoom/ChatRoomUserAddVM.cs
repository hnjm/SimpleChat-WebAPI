using SimpleChat.Core.Validation;
using SimpleChat.Core.ViewModel;
using System;
using System.ComponentModel.DataAnnotations;

namespace SimpleChat.ViewModel.ChatRoom
{
    public record ChatRoomUserAddVM : AddVM
    {
        [Required]
        [GuidValidation]
        public Guid UserId { get; set; }
        [Required]
        [GuidValidation]
        public Guid ChatRoomId { get; set; }
    }
}
