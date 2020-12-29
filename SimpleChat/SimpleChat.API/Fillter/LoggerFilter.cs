using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NGA.Core;
using NGA.Core.Enum;
using NGA.Core.Model;
using NGA.Core.Parameter;
using NGA.Core.Validation;
using NGA.Data;
using NGA.Domain;
using NGA.MonolithAPI.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NGA.MonolithAPI.Fillter
{
    public class LoggerFilter : IActionFilter
    {
        ILogger<LoggerFilter> _logger;

        public LoggerFilter(ILogger<LoggerFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                if (ParameterValue.SYS01001)
                {
                    string requestBody = "";

                    if (filterContext.ActionArguments != null || filterContext.ActionArguments.Count > 0)
                        requestBody = JsonConvert.SerializeObject(filterContext.ActionArguments.ToList());

                    LogVM model = LogHelper.GetVM(
                        ((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)filterContext.ActionDescriptor).ActionName,
                        ((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)filterContext.ActionDescriptor).ControllerName,
                        filterContext.HttpContext.Request.Method,
                        filterContext.HttpContext.Request.Host + filterContext.HttpContext.Request.Path,
                        requestBody,
                        ((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)filterContext.ActionDescriptor).MethodInfo.ReturnType.FullName);

                    _logger.LogInformation(model.ToString());
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, "Error in LogFilter");
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            try
            {
                if (ParameterValue.SYS01001)
                {
                 
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, "Error in LogFilter");
            }
        }
    }
}

