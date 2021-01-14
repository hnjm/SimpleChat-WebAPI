using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleChat.Core.ViewModel
{
    public interface IAddVM
    {
        //DateTime CreateDT { get; set; }
        //Guid CreateBy { get; set; }
    }
    public record AddVM : IAddVM
    {
        //public DateTime CreateDT { get; set; }
        //public Guid CreateBy { get; set; }
    }
}