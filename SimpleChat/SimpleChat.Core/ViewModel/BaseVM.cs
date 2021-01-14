using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleChat.Core.ViewModel
{
    public interface IBaseVM
    {
        Guid Id { get; set; }
    }
    public record BaseVM : IBaseVM
    {
        public Guid Id { get; set; }
    }
}