using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SimpleChat.Core.ViewModel
{
    public interface IResultVM
    {
        Guid? RecId { get; set; }

        object Rec { get; set; }
        bool IsSuccessful { get; set; }
        string StatusCode { get; set; }
    }

    public class APIResultVM : IResultVM
    {
        public Guid? RecId { get; set; }

        public object Rec { get; set; }
        public bool IsSuccessful { get; set; }
        public string StatusCode { get; set; }
    }
}