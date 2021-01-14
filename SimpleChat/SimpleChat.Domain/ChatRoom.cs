using SimpleChat.Core.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SimpleChat.Domain
{
    public record ChatRoomBase : TableEntity
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
    }

    public record ChatRoom : ChatRoomBase
    {
        //Foreign keys
        public virtual ICollection<ChatRoomUser> Users { get; set; }
        public virtual ICollection<Message> Messages { get; set; }

        public ChatRoom()
        {
            Users = new HashSet<ChatRoomUser>();
            Messages = new HashSet<Message>();
        }
    }
}
