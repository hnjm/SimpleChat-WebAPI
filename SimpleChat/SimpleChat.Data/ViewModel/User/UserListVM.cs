using SimpleChat.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleChat.Data.ViewModel.User
{
    public class UserListVM : BaseVM
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
    }
}
