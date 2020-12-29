using NGA.Core.Enum;
using NGA.Core.Parameter;
using NGA.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NGA.MonolithAPI.Logging
{
    public static class LogHelper
    {
        public static LogVM GetVM(string actionName, string controllerName,
            string httpMethodType, string path, string requestBody, string returnType)
        {
            if (!ParameterValue.SYS01001)
                return new LogVM();

            LogVM model = new LogVM();
            model.Id = Guid.NewGuid();
            model.CreateDate = DateTime.Now;
            model.RequestBody = requestBody;
            model.Path = Validation.IsNull(path) ? "-" : path;
            model.ActionName = Validation.IsNull(actionName) ? "-" : actionName;
            model.ControllerName = Validation.IsNull(controllerName) ? "-" : controllerName;
            model.ReturnTypeName = Validation.IsNull(returnType) ? "-" : returnType;
            model.MethodType = Validation.IsNull(httpMethodType) ? HTTPMethodType.Unknown :
                        (httpMethodType.ToUpper() == "POST" ? HTTPMethodType.POST :
                        (httpMethodType.ToUpper() == "PUT" ? HTTPMethodType.PUT :
                        (httpMethodType.ToUpper() == "DELETE" ? HTTPMethodType.DELETE :
                        (httpMethodType.ToUpper() == "GET" ? HTTPMethodType.GET : HTTPMethodType.Unknown))));            

            return model;
        }
    }
}
