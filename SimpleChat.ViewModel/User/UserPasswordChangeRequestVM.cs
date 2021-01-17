using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SimpleChat.ViewModel.User
{
    public class UserPasswordChangeRequestVM
    {
        [Required]
        [StringLength(100, MinimumLength = 5)]
        public string UserName { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 5)]
        public string EMail { get; set; }
    }
}
