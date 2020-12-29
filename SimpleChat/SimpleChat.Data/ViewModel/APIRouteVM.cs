using System;
using System.Collections.Generic;
using System.Text;

namespace NGA.Data.ViewModel
{
    public class APIRouteVM
    {
        public string Action { get; set; }
        public string Controller { get; set; }
        public string Path { get; set; }
        public string Parameters { get; set; }
    }
}
