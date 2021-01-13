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
using SimpleChat.Data.ViewModel.ChatRoom;
using SimpleChat.Domain;
using Microsoft.AspNetCore.Identity;

namespace SimpleChat.API.Controllers.V1
{
    /// <summary>
    /// CRUD operations about the Chat Rooms
    /// </summary>
    [ApiVersion("1.0")]
    public class ChatRoomsController : DefaultApiCRUDController<ChatRoomAddVM, ChatRoomUpdateVM, ChatRoomVM, ChatRoom, IChatRoomService>
    {
        #region Properties and Fields

        readonly UserManager<User> _userManager;

        #endregion

        #region Ctor

#pragma warning disable 1591
        public ChatRoomsController(IChatRoomService service,
            ILogger<ChatRoomsController> logger,
            UserManager<User> userManager)
             : base(service, logger)
        {
            _userManager = userManager;
        }
#pragma warning restore 1591

        #endregion

        /// <summary>
        ///  To get chat rooms for a User, returns only accessable chat roooms
        /// </summary>
        /// <returns>List of chat rooms</returns>
        /// <response code="404">If DB don't have any record for specific user</response>
        /// <response code="400">If the Current UserId is empty or null</response>
        /// <response code="200">If any records exist for the user on the DB</response>
        /// <response code="500">Empty payload with HTTP Status Code</response>
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(APIResultVM))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(APIResultVM))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResultWithRecVM<IEnumerable<ChatRoomVM>>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<JsonResult> Get()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if(user == null)
                new JsonAPIResult(APIResult.CreateVMWithStatusCode(false, null, APIStatusCode.ERR01004),
                    StatusCodes.Status403Forbidden);

            if (user.Id.IsEmptyGuid())
                return new JsonAPIResult(APIResult.CreateVMWithStatusCode(false, null, APIStatusCode.ERR01003),
                    StatusCodes.Status400BadRequest);

            var result = _service.GetByUserId(user.Id);
            if (result == null)
                return new JsonAPIResult(APIResult.CreateVMWithStatusCode(false, null, APIStatusCode.ERR01003),
                    StatusCodes.Status404NotFound);

            return new JsonAPIResult(result, StatusCodes.Status200OK);
            // return await (Task.Run(() =>
            // {
            //     new JsonAPIResult(APIResult.CreateVMWithStatusCode(false, null, APIStatusCode.ERR01009),
            //         StatusCodes.Status403Forbidden);
            // }) as Task<JsonResult>);
        }

        /// <summary>
        ///  To get users of the a chat room
        /// </summary>
        /// <returns>List of users guid ids of the chat room</returns>
        /// <response code="404">If the chat room dosent have any user</response>
        /// <response code="400">If the Id is empty or null</response>
        /// <response code="200">If any user exist for the chat room on the DB</response>
        /// <response code="500">Empty payload with HTTP Status Code</response>
        [HttpGet]
        [Route("{id}/users")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(APIResultVM))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(APIResultVM))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResultWithRecVM<IEnumerable<Guid>>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public JsonResult GetUsers([FromRoute] Guid id)
        {
            if (id.IsEmptyGuid())
                return new JsonAPIResult(APIResult.CreateVMWithStatusCode(false, null, APIStatusCode.ERR01003),
                    StatusCodes.Status400BadRequest);

            var result = _service.GetUsers(id);
            if (result == null)
                return new JsonAPIResult(APIResult.CreateVMWithStatusCode(false, null, APIStatusCode.ERR01002),
                    StatusCodes.Status404NotFound);

            return new JsonAPIResult(result, StatusCodes.Status200OK);
        }
    }
}