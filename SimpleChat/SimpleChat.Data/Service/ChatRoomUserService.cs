using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.Extensions.Logging;
using SimpleChat.Data.SubStructure;
using SimpleChat.Data.ViewModel.ChatRoom;
using SimpleChat.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleChat.Data.Service
{
    public class ChatRoomUserService : BaseService<ChatRoomUserAddVM, ChatRoomUserUpdateVM, ChatRoomUserVM, ChatRoomUser>, IChatRoomUserService
    {
        #region Ctor

        public ChatRoomUserService(UnitOfWork _uow, IMapper _mapper, ILogger<IRepository<ChatRoomUser>> _repositoryLogger)
            : base(_uow, _mapper, _repositoryLogger)
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
