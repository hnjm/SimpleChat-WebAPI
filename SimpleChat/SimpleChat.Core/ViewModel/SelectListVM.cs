using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleChat.Core.ViewModel
{
    public record SelectListVM
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
    }
}
