using SimpleChat.Core;
using SimpleChat.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SimpleChat.ViewModel.User
{
    public record UserUpdateVM : UpdateVM
    {
        [Required(ErrorMessage= APIStatusCode.ERR03001)]
        [MinLength(5, ErrorMessage= APIStatusCode.ERR03002)]
        [MaxLength(100, ErrorMessage= APIStatusCode.ERR03003)]
        public string DisplayName { get; set; }
        [MinLength(5, ErrorMessage= APIStatusCode.ERR03002)]
        [MaxLength(500, ErrorMessage= APIStatusCode.ERR03003)]
        public string About { get; set; }
    }
}
