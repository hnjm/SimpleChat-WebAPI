using SimpleChat.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleChat.ViewModel.User
{
    public record UserListVM : BaseVM
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
    }
}
