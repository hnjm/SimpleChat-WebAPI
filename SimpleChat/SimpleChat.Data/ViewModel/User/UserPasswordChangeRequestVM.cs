using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SimpleChat.Data.ViewModel.User
{
    public record UserPasswordChangeRequestVM
    {
        [Required]
        [StringLength(100, MinimumLength = 5)]
        public string UserName { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 5)]
        public string EMail { get; set; }
    }
}
