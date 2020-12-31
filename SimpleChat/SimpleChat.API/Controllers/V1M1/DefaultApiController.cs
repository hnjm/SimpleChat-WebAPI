using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SimpleChat.API.Controllers.V1M1
{
    //TODO: REFACTOR IT
    // [Authorize]
    //[ApiExplorerSettings(IgnoreApi = true)]
    [ApiVersion("1.1")]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public abstract class DefaultApiController : ControllerBase
    {

        protected ILogger<DefaultApiController> _logger;

        public DefaultApiController(ILogger<DefaultApiController> logger)
        {
            this._logger = logger;
        }
    }
}