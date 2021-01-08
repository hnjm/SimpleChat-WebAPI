using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SimpleChat.Core.ViewModel
{
    public interface IAPIResultVM
    {
        Guid? RecId { get; set; }
        bool IsSuccessful { get; set; }
        IEnumerable<APIResultErrorCodeVM> Errors { get; set; }
    }

    public class APIResultVM : IAPIResultVM
    {
        public Guid? RecId { get; set; }
        public bool IsSuccessful { get; set; }

        public IEnumerable<APIResultErrorCodeVM> Errors { get; set; }
    }
}