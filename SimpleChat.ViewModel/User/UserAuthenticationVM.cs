using SimpleChat.Core.ViewModel;
using SimpleChat.ViewModel.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleChat.ViewModel.User
{
    public class UserAuthenticationVM : BaseVM
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string About { get; set; }

        public TokenCacheVM TokenData { get; set; }
    }
}
