using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SimpleChat.Data.ViewModel.User
{
    public class UserPasswordChangeVM
    {
        [Required]
        [StringLength(36, MinimumLength = 36)]
        public string VerificationCode { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 10)]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 10)]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }
}
