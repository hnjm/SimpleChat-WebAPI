using SimpleChat.Core.EntityFramework;
using SimpleChat.Core.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SimpleChat.Domain
{
    public record ChatRoomUserBase : BaseEntity
    {
        [Required]
        [GuidValidation]
        public Guid UserId { get; set; }
        [Required]
        [GuidValidation]
        public Guid ChatRoomId { get; set; }
    }

    public record ChatRoomUser : ChatRoomUserBase
    {
        //Foreign keys
        public virtual ChatRoom ChatRoom { get; set; }
        public virtual User User { get; set; }

        public ChatRoomUser()
        {
        }
    }
}
