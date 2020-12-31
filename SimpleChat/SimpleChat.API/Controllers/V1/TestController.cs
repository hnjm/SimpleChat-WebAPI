using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SimpleChat.API.Controllers.V1
{
    [ApiVersion("1.0")]
    public class TestController : DefaultApiController
    {
        public TestController(ILogger<TestController> logger)
             : base(logger)
        {
        }

        [HttpGet]
        public JsonResult Version()
        {
            return new JsonResult("V1.0");
        }
    }
}