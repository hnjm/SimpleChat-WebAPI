using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SimpleChat.Domain
{
    public class RoleBase : IdentityRole<Guid>
    {
        [Required]
        [DefaultValue(false)]
        public bool CanManageGroups { get; set; }
        [Required]
        [DefaultValue(false)]
        public bool CanJoinAnyGroup { get; set; }

    }

    [Table("Roles")]
    public class Role : RoleBase
    {
        //Foreign keys
        public virtual ICollection<User> Users { get; set; }

        public Role()
        {
            Users = new HashSet<User>();
        }
    }
}
