using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleChat.Core;
using SimpleChat.Core.Helper;
using SimpleChat.Data.Service;
using SimpleChat.Data.ViewModel.ChatRoom;
using SimpleChat.Domain;

namespace SimpleChat.API.Controllers.V1
{
    [ApiVersion("1.0")]
    public class ChatRoomController : DefaultApiCRUDController<ChatRoomAddVM, ChatRoomUpdateVM, ChatRoomVM, ChatRoom, ChatRoomService>
    {
        public ChatRoomController(ChatRoomService service, ILogger<ChatRoomController> logger)
             : base(service, logger)
        {

        }

        [HttpGet]
        public virtual JsonResult GetByUserId(Guid userId)
        {
            try
            {
                var result = _service.GetByUserId(userId);

                if (result == null)
                    return new JsonResult(APIResult.CreateVM(false, null, APIStatusCode.WRG01001));

                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return new JsonResult(APIResult.CreateVM());

            }
        }

        [HttpGet]
        public virtual JsonResult GetUsers(Guid groupId)
        {
            try
            {
                var result = _service.GetUsers(groupId);

                if (result == null)
                    return new JsonResult(APIResult.CreateVM(false, null, APIStatusCode.WRG01001));

                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return new JsonResult(APIResult.CreateVM());
            }
        }
    }
}