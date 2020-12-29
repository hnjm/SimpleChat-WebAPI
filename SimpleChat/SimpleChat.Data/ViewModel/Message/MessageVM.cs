using SimpleChat.Core.Validation;
using SimpleChat.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SimpleChat.Data.ViewModel.Message
{
    public class MessageVM : TableEntityVM 
    {
        public string Text { get; set; }
        public Guid ChatRoomId { get; set; }
    }

    public class MessageAddVM : AddVM
    {
        [Required]
        [MinLength(5)]
        [MaxLength(500)]
        public string Text { get; set; }
        [Required]
        [GuidValidation]
        public Guid ChatRoomId { get; set; }
    }

    public class MessageUpdateVM : UpdateVM
    {
        [Required]
        [MinLength(5)]
        [MaxLength(500)]
        public string Text { get; set; }
    }
}
