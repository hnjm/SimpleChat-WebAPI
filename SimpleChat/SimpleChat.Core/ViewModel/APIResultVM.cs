using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SimpleChat.Core.ViewModel
{
    public interface IIsResultVM
    {
        bool IsSuccessful { get; set; }
    }

    public class APIResultVM : IIsResultVM
    {
        public Guid? RecId { get; set; }

        public object Rec { get; set; }
        public bool IsSuccessful { get; set; }
        public string StatusCode { get; set; }
    }
}