using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SimpleChat.Data.ViewModel.User
{
    public class UserLoginVM
    {
        [Required, MaxLength(15)]
        public string UserName { get; set; }
        [Required, MaxLength(50)]
        public string Password { get; set; }
    }
}
