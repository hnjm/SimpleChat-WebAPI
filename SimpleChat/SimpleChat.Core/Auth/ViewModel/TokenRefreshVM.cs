using System;
using System.ComponentModel.DataAnnotations;

namespace SimpleChat.Core.Auth.ViewModel
{
    public record TokenRefreshVM
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        public string RefreshToken { get; set; }
    }
}
