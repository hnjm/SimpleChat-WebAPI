﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using SimpleChat.Core.ViewModel;

namespace SimpleChat.ViewModel.SignalR
{
    public record SendMessageVM
    {
        public string Text { get; set; }
    }
}
