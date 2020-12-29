using SimpleChat.Core.Validation;
using SimpleChat.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NGA.Data.ViewModel.ChatRoom
{
    public class ChatRoomUserVM : BaseVM 
    {
        public Guid UserId { get; set; }
        public Guid ChatRoomId { get; set; }
    }

    public class ChatRoomUserAddVM : AddVM
    {
        [Required]
        [GuidValidation]
        public Guid UserId { get; set; }
        [Required]
        [GuidValidation]
        public Guid ChatRoomId { get; set; }
    }

    public class ChatRoomUserUpdateVM : UpdateVM
    {
        [Required]
        [GuidValidation]
        public Guid UserId { get; set; }
        [Required]
        [GuidValidation]
        public Guid ChatRoomId { get; set; }
    }
}
