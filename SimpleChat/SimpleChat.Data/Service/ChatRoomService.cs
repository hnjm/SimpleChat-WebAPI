using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.Extensions.Logging;
using SimpleChat.Data.SubStructure;
using SimpleChat.Data.ViewModel.ChatRoom;
using SimpleChat.Domain;
using SimpleChat.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleChat.Core.ViewModel;
using SimpleChat.Core;
using SimpleChat.Core.Helper;

namespace SimpleChat.Data.Service
{
    public class ChatRoomService : BaseService<ChatRoomAddVM, ChatRoomUpdateVM, ChatRoomVM, ChatRoom>, IChatRoomService
    {
        #region Ctor
        private IChatRoomUserService _chatRoomUserService;

        public ChatRoomService(UnitOfWork uow,
            IMapper mapper,
            ILogger<IRepository<ChatRoom>> repositoryLogger,
            IChatRoomUserService chatRoomUserService)
            : base(uow, mapper, repositoryLogger)
        {
            _chatRoomUserService = chatRoomUserService;
        }

        #endregion

        #region Methods


        public List<ChatRoomVM> GetByUserId(Guid userId)
        {
            if (userId.IsEmptyGuid())
            {
                return new List<ChatRoomVM>();
            }

            var ChatRooms = Query().Where(a => !a.IsPrivate
                || (a.Users.Any(x => x.UserId == userId)))
                .AsEnumerable().Select(a => new ChatRoomVM()
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                IsPrivate = a.IsPrivate,
                IsMain = a.IsMain,
                IsOneToOneChat = a.IsOneToOneChat,
                    Users = _chatRoomUserService.Query().Where(x => x.ChatRoomId == a.Id)
                    .Select(s => s.UserId).ToList()
            }).ToList();

            return ChatRooms;
        }

        public List<Guid> GetUsers(Guid chatRoomId)
        {
            if (chatRoomId.IsEmptyGuid())
            {
                return new List<Guid>();
            }

            var users = _chatRoomUserService.Query()
                .Where(a => a.ChatRoomId == chatRoomId)
                .Select(a => a.UserId).ToList();

            return users;
        }

        public override async Task<ChatRoomVM> GetByIdAsync(Guid id)
        {
            var rec = await base.GetByIdAsync(id);

            if (rec != null)
            {
                rec.Users = _chatRoomUserService.Query()
                    .Where(a => a.ChatRoomId == id)
                    .Select(a => a.Id).ToList();
            }

            return rec;
        }

        public override async Task<IAPIResultVM> AddAsync(ChatRoomAddVM model, Guid? userId = null, bool isCommit = true)
        {
            if (model.IsMain)
            {
                model.IsMain = false;
            }

            var result = await base.AddAsync(model, userId, true);
            if (result.ResultIsNotTrue())
                return result;

            if (model.Users != null && model.Users.Count > 0)
            {
                foreach (var id in model.Users)
                {
                    ChatRoomUserAddVM item = new ChatRoomUserAddVM();
                    item.ChatRoomId = result.RecId.Value;
                    item.UserId = id;

                    var userResult = await _chatRoomUserService.AddAsync(item, userId, true);
                    if (userResult.ResultIsNotTrue())
                        return userResult;
                }
            }

            return result;
        }

        public override async Task<IAPIResultVM> UpdateAsync(Guid id, ChatRoomUpdateVM model, Guid? userId = null, bool isCommit = true)
        {
            if (model.IsMain && Repository.Query().Any(a => a.Id != id && a.IsMain))
                return APIResult.CreateVMWithStatusCode(false, id, APIStatusCode.ERR01005);

            var result = await base.UpdateAsync(id, model, userId, true);
            if (result.ResultIsNotTrue())
                return result;

            if (model.Users != null && model.Users.Count > 0)
            {
                foreach (var item in model.Users)
                {
                    bool isExist = await _chatRoomUserService.AnyAsync(a => a.ChatRoomId == id && a.UserId == item);
                    if (!isExist)
                    {
                        ChatRoomUserAddVM rec = new ChatRoomUserAddVM();
                        rec.ChatRoomId = id;
                        rec.UserId = item;

                        var userResult = await _chatRoomUserService.AddAsync(rec, userId, true);
                        if (userResult.ResultIsNotTrue())
                            return userResult;
                    }
                }
            }

            var removedUsers = _chatRoomUserService.Query().Where(a => a.ChatRoomId == id
                && (model.Users==null || !model.Users.Any(x => x == a.UserId))).Select(a => a.Id).ToList();
            if (removedUsers != null)
            {
                foreach (var item in removedUsers)
                {
                    await _chatRoomUserService.DeleteAsync(item, userId, false, true);
                }
            }

            return result;
        }

        public override async Task<IAPIResultVM> DeleteAsync(Guid id, Guid? userId = null,
            bool shouldBeOwner = false, bool isCommit = true)
        {
            var ChatRoom = await base.GetByIdAsync(id);
            if (ChatRoom == null || ChatRoom.IsMain)
                return APIResult.CreateVMWithStatusCode(false, id, APIStatusCode.ERR01005);


            var result = await base.DeleteAsync(id, userId, shouldBeOwner, true);
            if (result.ResultIsNotTrue())
                return result;

            var users = _chatRoomUserService.Query().Where(a => a.ChatRoomId == id).Select(a => a.Id).ToList();
            if (users != null)
            {
                foreach (var item in users)
                {
                    await _chatRoomUserService.DeleteAsync(item, userId, false, true);
                }
            }

            return result;
        }

        #endregion
    }

    public interface IChatRoomService : IBaseService<ChatRoomAddVM, ChatRoomUpdateVM, ChatRoomVM, ChatRoom>
    {
        List<ChatRoomVM> GetByUserId(Guid userId);
        List<Guid> GetUsers(Guid ChatRoomId);
    }
}
