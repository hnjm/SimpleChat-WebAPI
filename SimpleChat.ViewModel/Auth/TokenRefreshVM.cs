using System;
using System.ComponentModel.DataAnnotations;
using SimpleChat.Core;

namespace SimpleChat.ViewModel.Auth.ViewModel
{
    public record TokenRefreshVM
    {
        [Required(ErrorMessage= APIStatusCode.ERR03001)]
        public string AccessToken { get; set; }
        [Required(ErrorMessage= APIStatusCode.ERR03001)]
        public string RefreshToken { get; set; }
    }
}
