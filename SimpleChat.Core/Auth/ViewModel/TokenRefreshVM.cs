using System;
using System.ComponentModel.DataAnnotations;

namespace SimpleChat.Core.Auth.ViewModel
{
    public class TokenRefreshVM
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        public string RefreshToken { get; set; }
    }
}
