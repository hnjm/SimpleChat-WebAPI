using Microsoft.AspNetCore.Identity;
using SimpleChat.Core.Enum;
using SimpleChat.Core.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SimpleChat.Domain
{
    public class UserBase : IdentityUser<Guid>
    {
        [Required]
        [MinLength(5)]
        [MaxLength(100)]
        public string DisplayName { get; set; }

        [MinLength(5)]
        [MaxLength(500)]
        public string About { get; set; }

        [Required]
        [GuidValidation]
        public Guid RoleId { get; set; }

        [Required]
        public DateTime CreateDateTime { get; set; }

        public DateTime? LastLoginDateTime { get; set; }
    }

    public class User : UserBase
    {
        //Foreign keys
        public virtual Role Role { get; set; }

        public virtual ICollection<ChatRoomUser> ChatRooms { get; set; }

        public User()
        {
            ChatRooms = new HashSet<ChatRoomUser>();
        }
    }
}
