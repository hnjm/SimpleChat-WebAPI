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
    [ApiVersion("1.0")]
    public class MessagesController : DefaultApiCRUDController<MessageAddVM, MessageUpdateVM, MessageVM, Message, MessageService>
    {
        public MessagesController(MessageService service,
            ILogger<MessagesController> logger)
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
                    return new JsonResult(APIResult.CreateVMWithStatusCode(false, null, APIStatusCode.ERR01002));

                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return new JsonResult(APIResult.CreateVM());
            }
        }
    }
}