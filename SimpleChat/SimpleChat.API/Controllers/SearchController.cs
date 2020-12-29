using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NGA.Data.Service;
using NGA.Domain;

namespace NGA.MonolithAPI.Controllers.V2
{
    public class SearchController : DefaultApiController
    {
        private IMessageService _service;

        public SearchController(IMessageService service, ILogger<SearchController> logger)
             : base(logger)
        {
            this._service = service;
        }

        [HttpGet]
        public JsonResult Get(string key)
        {
            if(key == null || key == "" || key.Length < 4)
            {
                return new JsonResult("");
            }

            var messages = _service.Repository.Query().Where(s => s.Text.Contains(key)).ToList();

            return new JsonResult(messages);
        }
    }
}