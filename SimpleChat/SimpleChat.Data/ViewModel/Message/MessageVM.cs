using SimpleChat.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SimpleChat.Data.ViewModel.Message
{
    public class MessageVM : TableEntityVM 
    {
        public string Text { get; set; }
        public Guid ChatRoomId { get; set; }
    }
}
