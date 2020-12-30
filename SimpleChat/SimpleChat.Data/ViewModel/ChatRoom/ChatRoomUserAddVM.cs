﻿using SimpleChat.Core.Validation;
using SimpleChat.Core.ViewModel;
using System;
using System.ComponentModel.DataAnnotations;

namespace SimpleChat.Data.ViewModel.ChatRoom
{
    public class ChatRoomUserAddVM : AddVM
    {
        [Required]
        [GuidValidation]
        public Guid UserId { get; set; }
        [Required]
        [GuidValidation]
        public Guid ChatRoomId { get; set; }
    }
}