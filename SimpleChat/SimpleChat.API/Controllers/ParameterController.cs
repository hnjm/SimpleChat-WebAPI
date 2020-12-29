using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NGA.Core;
using NGA.Core.Helper;
using NGA.Core.Model;
using NGA.Data;
using NGA.Data.Service;
using NGA.Data.ViewModel;
using NGA.Domain;

namespace NGA.MonolithAPI.Controllers.V2
{
    public class ParameterController : DefaultApiCRUDController<ParameterAddVM, ParameterUpdateVM, ParameterVM, IParameterService>
    {
        public ParameterController(IParameterService service, ILogger<ParameterController> logger)
             : base(service, logger)
        {

        }
        
        [HttpPost]
        public override Task<JsonResult> Add(ParameterAddVM model)
        {
            return base.Add(null);
        }

        [HttpPut]
        public override Task<JsonResult> Update(Guid id, ParameterUpdateVM model)
        {
            return base.Update(Guid.Empty, null);
        }

        [HttpDelete]
        public override Task<JsonResult> Delete(Guid id)
        {
            return base.Delete(Guid.Empty);
        }
    }
}
