using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleChat.Data.Service;
using SimpleChat.Domain;

namespace SimpleChat.API.Controllers.V1
{
    [ApiVersion("1.0")]
    public class SearchesController : DefaultApiController
    {
        private IMessageService _service;

        public SearchesController(IMessageService service,
            ILogger<SearchesController> logger)
             : base(logger)
        {
            this._service = service;
        }

        /// <summary>
        /// Get a list of messages which is contains 'key' parameter
        /// </summary>
        /// <param name="key">Text fields of the messages querying by this paramerter</param>
        /// <returns>A list of filtered messages</returns>
        /// <response code="200">Returns a list of message data class</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Message>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        public IActionResult Get(string key)
        {
            if(key == null || key == "" || key.Length < 4)
            {
                return BadRequest();
            }

            var messages = _service.Query().Where(s => s.Text.Contains(key)).ToList();

            return Ok(messages);
        }
    }
}