using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleChat.Core;
using SimpleChat.Core.Helper;
using SimpleChat.Core.Validation;
using SimpleChat.Core.ViewModel;
using SimpleChat.Data.Service;
using SimpleChat.Data.ViewModel.Message;
using SimpleChat.Domain;

namespace SimpleChat.API.Controllers.V1
{
    /// <summary>
    /// Using purpose is making operation on a lot of data,
    /// if we want to send message or we want to make any operation about only one message record,
    /// we have to use SignalR Hub
    /// </summary>
    [ApiVersion("1.0")]
    public class MessagesController : DefaultApiCRUDController<MessageAddVM, MessageUpdateVM, MessageVM, Message, IMessageService>
    {
        #region Properties and Fields



        #endregion

        #region Ctor

#pragma warning disable 1591
        public MessagesController(IMessageService service,
            ILogger<MessagesController> logger)
             : base(service, logger)
        {

        }
#pragma warning restore 1591

        #endregion

        /// <summary>
        /// NOT IMPLEMENTED
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public override async Task<JsonResult> Get()
        {
            return new JsonAPIResult(APIResult.CreateVMWithStatusCode(false, null),
                                    StatusCodes.Status501NotImplemented);
        }

        /// <summary>
        /// Returns messages of a group, that depends to {id} parameter
        /// </summary>
        /// <param name="id">ID value of the Chat Room</param>
        /// <returns>List of messages for a specific chat room</returns>
        /// <response code="404">If the chat room dosent have any message</response>
        /// <response code="400">If the Id is empty or null</response>
        /// <response code="200">If any message exist for the chat room on the DB</response>
        /// <response code="500">Empty payload with HTTP Status Code</response>
        [HttpGet]
        [Route("/api/chatrooms/{id}/messages")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(APIResultVM))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(APIResultVM))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResultWithRecVM<IEnumerable<MessageVM>>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public virtual JsonResult GetByChatRoomId([FromRoute] Guid id)
        {
            if (id.IsEmptyGuid())
                return new JsonAPIResult(APIResult.CreateVMWithStatusCode(false, null, APIStatusCode.ERR01003),
                    StatusCodes.Status400BadRequest);

            var result = _service.GetMessagesByChatRoomId(id);
            if (result == null)
                return new JsonAPIResult(APIResult.CreateVMWithStatusCode(false, null, APIStatusCode.ERR01002),
                    StatusCodes.Status404NotFound);

            return new JsonAPIResult(result, StatusCodes.Status200OK);
        }

        /// <summary>
        /// NOT IMPLEMENTED
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override async Task<JsonResult> Add([FromBody] MessageAddVM model)
        {
            return new JsonAPIResult(APIResult.CreateVMWithStatusCode(false, null),
                    StatusCodes.Status501NotImplemented);
        }
    }
}