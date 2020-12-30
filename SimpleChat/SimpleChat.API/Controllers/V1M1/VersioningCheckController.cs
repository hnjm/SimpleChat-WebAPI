using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SimpleChat.API.Controllers.V1M1
{
    [ApiVersion("1.1")]
    public class VersioningController : DefaultApiController
    {
        public VersioningController(ILogger<VersioningController> logger)
             : base(logger)
        {
        }

        [HttpGet]
        public JsonResult Get()
        {
            return new JsonResult("V1.1");
        }
    }
}