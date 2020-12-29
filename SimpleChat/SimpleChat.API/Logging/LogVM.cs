using NGA.Core.Enum;
using NGA.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NGA.MonolithAPI.Logging
{
    public class LogVM : BaseVM
    {
        public DateTime CreateDate { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string ReturnTypeName { get; set; }
        public string Path { get; set; }
        public HTTPMethodType MethodType { get; set; }
        public string RequestBody { get; set; }

        public override string ToString()
        {
            return $@"CREATE-DATE: {CreateDate.ToString()};
                      CONTROLLER: {ControllerName};
                      ACTION: {ActionName};
                      RETURN-TYPE: {ReturnTypeName};
                      PATH: {Path};
                      HTTP: {nameof(MethodType)};
                      REQUEST-BODY: {RequestBody};";
        }
    }
}
