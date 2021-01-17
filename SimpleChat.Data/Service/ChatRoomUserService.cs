using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.Extensions.Logging;
using SimpleChat.Data.SubStructure;
using SimpleChat.ViewModel.ChatRoom;
using SimpleChat.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleChat.Core.Helper;

namespace SimpleChat.Data.Service
{
    public class ChatRoomUserService : BaseService<ChatRoomUserAddVM, ChatRoomUserUpdateVM, ChatRoomUserVM, ChatRoomUser>, IChatRoomUserService
    {
        #region Ctor

        public ChatRoomUserService(UnitOfWork uow,
            IMapper mapper,
            APIResult apiResult)
            : base(uow, mapper, apiResult)
        {

        }

        #endregion

        #region Methods


        #endregion
    }

    public interface IChatRoomUserService : IBaseService<ChatRoomUserAddVM, ChatRoomUserUpdateVM, ChatRoomUserVM, ChatRoomUser>
    {
    }
}
