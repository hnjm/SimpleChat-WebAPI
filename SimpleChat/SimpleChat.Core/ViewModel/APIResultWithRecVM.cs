using System;
using System.Collections.Generic;

namespace SimpleChat.Core.ViewModel
{
    public interface IResultWithRecVM<T> : IAPIResultVM
    {
        T Rec { get; set; }
    }
    public class APIResultWithRecVM<T> : IResultWithRecVM<T>
    {
        public Guid? RecId { get; set; }
        public T Rec { get; set; }
        public bool IsSuccessful { get; set; }

        public IEnumerable<APIResultErrorCodeVM> Errors { get; set; }
    }
}