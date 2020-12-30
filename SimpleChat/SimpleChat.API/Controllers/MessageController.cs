using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleChat.Core;
using SimpleChat.Core.Helper;
using SimpleChat.Data.Service;
using SimpleChat.Data.ViewModel.Message;
using SimpleChat.Domain;

namespace SimpleChat.API.Controllers.V1
{
    public class MessageController : DefaultApiCRUDController<MessageAddVM, MessageUpdateVM, MessageVM, Message, MessageService>
    {
        public MessageController(MessageService service, ILogger<MessageController> logger)
             : base(service, logger)
        {

        }

        [HttpGet]
        //Return messages of a group, which group ID sent
        public virtual JsonResult GetByChatRoomId(Guid groupId)
        {
            try
            {
                var result = _service.GetMessagesByChatRoomId(groupId);

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