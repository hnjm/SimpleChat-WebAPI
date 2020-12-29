using SimpleChat.Core.EntityFramework;
using SimpleChat.Core.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SimpleChat.Domain
{
    public class MessageBase : TableEntity
    {
        [Required]
        [MinLength(5)]
        [MaxLength(500)]
        public string Text { get; set; }
        [Required]
        [GuidValidation]
        public Guid ChatRoomId { get; set; }
    }

    public class Message : MessageBase
    {
        //Foreign keys
        public virtual ChatRoom ChatRoom { get; set; }

        public Message()
        {
        }
    }
}
