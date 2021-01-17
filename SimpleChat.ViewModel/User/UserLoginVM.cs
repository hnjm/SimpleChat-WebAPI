using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using SimpleChat.Core;

namespace SimpleChat.ViewModel.User
{
    public class UserLoginVM
    {
        [Required(ErrorMessage= APIStatusCode.ERR03001)]
        [MaxLength(15, ErrorMessage= APIStatusCode.ERR03003)]
        public string UserName { get; set; }
        [Required(ErrorMessage= APIStatusCode.ERR03001)]
        [MaxLength(50, ErrorMessage= APIStatusCode.ERR03003)]
        public string Password { get; set; }
    }
}
