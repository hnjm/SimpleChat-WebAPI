using SimpleChat.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SimpleChat.ViewModel.User
{
    public record UserVM : BaseVM
    {
        public string UserName { get; set; }

        public DateTime? LastLoginDateTime { get; set; }
        public bool IsAccountLocked { get; set; }

        public string DisplayName { get; set; }
        public string About { get; set; }
    }
}
