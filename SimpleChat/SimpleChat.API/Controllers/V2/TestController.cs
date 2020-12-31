using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SimpleChat.API.Controllers.V2
{
    [ApiVersion("2.0")]
    public class TestController : DefaultApiController
    {
        public TestController(ILogger<TestController> logger)
             : base(logger)
        {
        }

        [HttpGet]
        public JsonResult Version()
        {
            return new JsonResult("V2.0");
        }

        /// <summary>
        /// Gets a customized hi message
        /// </summary>
        /// <param name="model">Model contatins text field, which is conbines with the 'hi' message</param>
        /// <returns>Returns hi message</returns>
        /// <response code="200">Returns hi message</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost, Consumes("application/json")]
        public IActionResult ModelValidation(TestModel model)
        {
            if(model == null || model.Text == "" || model.Text.Length < 4)
            {
                return BadRequest();
            }

            return Ok(model.Text + " hi");
        }
    }

    public class TestModel
    {
        [MaxLength(10)]
        public string Text { get; set; }
    }
}