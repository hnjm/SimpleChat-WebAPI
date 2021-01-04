using SimpleChat.Core.ViewModel;
using System;
using System.ComponentModel.DataAnnotations;

namespace SimpleChat.Data.ViewModel.User
{
    public class UserRegisterVM : BaseVM
    {
        [Required]
        [MinLength(5)]
        [MaxLength(100)]
        public string DisplayName { get; set; }

        [MaxLength(500)]
        public string About { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(100)]
        public string Password { get; set; }
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(200)]
        public string Email { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(100)]
        public string UserName { get; set; }
    }
}
