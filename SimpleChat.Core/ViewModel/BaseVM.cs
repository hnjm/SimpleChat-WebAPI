using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleChat.Core.ViewModel
{
    public interface IBaseVM<T>
    {
        T Id { get; set; }
    }
    public record BaseVM<T> : IBaseVM<T>
    {
        public T Id { get; set; }
    }

    public interface IBaseVM : IBaseVM<Guid>
    {
    }
    public record BaseVM : BaseVM<Guid>, IBaseVM
    {
    }
}