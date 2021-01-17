using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SimpleChat.API.Controllers.V1
{
    /// <summary>
    /// Default controller, it contains basic functionality of the API Controller
    /// </summary>
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public abstract class DefaultApiController : ControllerBase
    {
        #region Properties and Fields



        #endregion

        #region Ctor

#pragma warning disable 1591

        public DefaultApiController()
        {
        }
        #pragma warning restore 1591

        #endregion
    }
}