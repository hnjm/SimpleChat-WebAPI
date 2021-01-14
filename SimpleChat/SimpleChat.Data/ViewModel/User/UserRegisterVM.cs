using SimpleChat.Core.ViewModel;
using SimpleChat.Core;
using System;
using System.ComponentModel.DataAnnotations;

namespace SimpleChat.Data.ViewModel.User
{
    public record UserRegisterVM : BaseVM
    {
        [Required(ErrorMessage= APIStatusCode.ERR03001)]
        [MinLength(5, ErrorMessage= APIStatusCode.ERR03002)]
        [MaxLength(100, ErrorMessage= APIStatusCode.ERR03003)]
        public string DisplayName { get; set; }

        [MaxLength(500, ErrorMessage= APIStatusCode.ERR03003)]
        public string About { get; set; }

        [Required(ErrorMessage= APIStatusCode.ERR03001)]
        [MinLength(8, ErrorMessage= APIStatusCode.ERR03002)]
        [MaxLength(100, ErrorMessage= APIStatusCode.ERR03003)]
        public string Password { get; set; }
        [Compare("Password", ErrorMessage= APIStatusCode.ERR03004)]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage= APIStatusCode.ERR03001)]
        [MinLength(5, ErrorMessage= APIStatusCode.ERR03002)]
        [MaxLength(200, ErrorMessage= APIStatusCode.ERR03003)]
        public string Email { get; set; }

        [Required(ErrorMessage= APIStatusCode.ERR03001)]
        [MinLength(5, ErrorMessage= APIStatusCode.ERR03002)]
        [MaxLength(100, ErrorMessage= APIStatusCode.ERR03003)]
        public string UserName { get; set; }
    }
}
