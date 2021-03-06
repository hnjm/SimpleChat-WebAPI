﻿using SimpleChat.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SimpleChat.ViewModel.ChatRoom
{
    public record ChatRoomUpdateVM : UpdateVM
    {
        [Required]
        [MinLength(5)]
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(250)]
        public string Description { get; set; }
        [Required]
        [DefaultValue(false)]
        public bool IsMain { get; set; }
        [Required]
        [DefaultValue(false)]
        public bool IsPrivate { get; set; }
        [Required]
        public bool IsOneToOneChat { get; set; }

        public List<Guid> Users { get; set; }
    }
}
