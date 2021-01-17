using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SimpleChat.Core.ViewModel
{
    public interface IAPIResultVM<IDType>
    {
        IDType RecId { get; set; }
        bool IsSuccessful { get; set; }
        IEnumerable<APIResultErrorCodeVM> Errors { get; set; }
    }

    public class APIResultVM<IDType> : IAPIResultVM<IDType>
    {
        public IDType RecId { get; set; }
        public bool IsSuccessful { get; set; }

        public IEnumerable<APIResultErrorCodeVM> Errors { get; set; }
    }

    public interface IAPIResultVM :  IAPIResultVM<Guid?>
    {
    }

    public class APIResultVM : APIResultVM<Guid?>, IAPIResultVM
    {
    }
}