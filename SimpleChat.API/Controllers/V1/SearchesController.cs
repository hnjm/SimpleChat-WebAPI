using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleChat.Core;
using SimpleChat.Core.Helper;
using SimpleChat.Core.ViewModel;
using SimpleChat.Data.Service;
using SimpleChat.ViewModel.Message;
using SimpleChat.Domain;

namespace SimpleChat.API.Controllers.V1
{
    /// <summary>
    /// Contains functionality of search operations
    /// </summary>
    [ApiVersion("1.0")]
    public class SearchesController : DefaultApiController
    {
        #region Properties and Fields

        /// <summary>
        /// Maps the objects one type to other type
        /// </summary>
        protected readonly IMapper _mapper;
        private readonly APIResult _apiResult;

        #endregion

        #region Ctor

#pragma warning disable 1591
        private IMessageService _service;

        public SearchesController(IMessageService service,
            IMapper mapper,
            APIResult apiResult)
        {
            _service = service;
            _mapper = mapper;
            _apiResult = apiResult;
        }
#pragma warning restore 1591

        #endregion

        /// <summary>
        /// Get a list of messages which is contains 'key' parameter
        /// </summary>
        /// <param name="key">Text fields of the messages querying by this paramerter</param>
        /// <returns>A list of filtered messages</returns>
        /// <response code="400">When KEY parameter is empty or null</response>
        /// <response code="404">If there is no record which is contains KEY parameter value</response>
        /// <response code="200">List of messages</response>
        /// <response code="500">Empty payload with HTTP Status Code</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(APIResultVM))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(APIResultVM))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResultWithRecVM<IEnumerable<MessageVM>>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public JsonResult Get([FromQuery] string key)
        {
            if(key == null || key == "" || key.Length < 4)
                return new JsonAPIResult(_apiResult.CreateVMWithStatusCode(null, false, APIStatusCode.ERR01003),
                    StatusCodes.Status400BadRequest);

            var messages = _service.Query().Where(s => s.Text.Contains(key));
            if (messages == null)
                return new JsonAPIResult(_apiResult.CreateVMWithStatusCode(null, false, APIStatusCode.ERR01002),
                    StatusCodes.Status404NotFound);

            var result = _mapper.ProjectTo<MessageVM>(messages).ToList();

            return new JsonAPIResult(result, StatusCodes.Status200OK);
        }
    }
}