using SimpleChat.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleChat.Data.ViewModel.User
{
    public class UserAuthenticationVM : BaseVM
    {
        public string UserName { get; set; }

        public DateTime? LastLoginDateTime { get; set; }

        public string DisplayName { get; set; }
        public string About { get; set; }

        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
