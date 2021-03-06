﻿using SimpleChat.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SimpleChat.ViewModel.User
{
    public record UserUpdateVM : UpdateVM
    {
        [Required]
        [MinLength(5)]
        [MaxLength(100)]
        public string DisplayName { get; set; }
        [MinLength(5)]
        [MaxLength(500)]
        public string About { get; set; }
    }
}
