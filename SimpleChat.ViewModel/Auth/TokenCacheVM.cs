using System;
using System.Collections.Generic;
using System.Text;
using SimpleChat.Core.ViewModel;

namespace SimpleChat.ViewModel.Auth
{
    public record TokenCacheVM : BaseVM
    {
        public string AccessToken { get; set; }
        public DateTime AccessTokenExpiryTime { get; set; }

        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
