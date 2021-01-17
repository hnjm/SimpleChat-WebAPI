using AutoMapper;
using SimpleChat.ViewModel;
using SimpleChat.ViewModel.ChatRoom;
using SimpleChat.ViewModel.Message;
using SimpleChat.ViewModel.User;
using SimpleChat.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleChat.Data
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region Chat Room User
            CreateMap<ChatRoom, ChatRoomVM>().ForMember(x => x.Users, opt => opt.Ignore());
            CreateMap<ChatRoom, ChatRoomAddVM>().ForMember(x => x.Users, opt => opt.Ignore());
            CreateMap<ChatRoom, ChatRoomUpdateVM>().ForMember(x => x.Users, opt => opt.Ignore());

            CreateMap<ChatRoomVM, ChatRoom>().ForMember(x => x.Users, opt => opt.Ignore());
            CreateMap<ChatRoomVM, ChatRoomAddVM>();
            CreateMap<ChatRoomVM, ChatRoomUpdateVM>();

            CreateMap<ChatRoomAddVM, ChatRoom>().ForMember(x => x.Users, opt => opt.Ignore());
            CreateMap<ChatRoomAddVM, ChatRoomVM>();
            CreateMap<ChatRoomAddVM, ChatRoomUpdateVM>();

            CreateMap<ChatRoomUpdateVM, ChatRoom>().ForMember(x => x.Users, opt => opt.Ignore());
            CreateMap<ChatRoomUpdateVM, ChatRoomVM>();
            CreateMap<ChatRoomUpdateVM, ChatRoomAddVM>();
            #endregion

            #region Chat Room User
            CreateMap<ChatRoomUser, ChatRoomUserVM>();
            CreateMap<ChatRoomUser, ChatRoomUserAddVM>();
            CreateMap<ChatRoomUser, ChatRoomUserUpdateVM>();

            CreateMap<ChatRoomUserVM, ChatRoomUser>();
            CreateMap<ChatRoomUserVM, ChatRoomUserAddVM>();
            CreateMap<ChatRoomUserVM, ChatRoomUserUpdateVM>();

            CreateMap<ChatRoomUserAddVM, ChatRoomUser>();
            CreateMap<ChatRoomUserAddVM, ChatRoomUserVM>();
            CreateMap<ChatRoomUserAddVM, ChatRoomUserUpdateVM>();

            CreateMap<ChatRoomUserUpdateVM, ChatRoomUser>();
            CreateMap<ChatRoomUserUpdateVM, ChatRoomUserVM>();
            CreateMap<ChatRoomUserUpdateVM, ChatRoomUserAddVM>();
            #endregion

            #region Message
            CreateMap<Message, MessageVM>();
            CreateMap<Message, MessageAddVM>();
            CreateMap<Message, MessageUpdateVM>();

            CreateMap<MessageVM, Message>();
            CreateMap<MessageVM, MessageAddVM>();
            CreateMap<MessageVM, MessageUpdateVM>();

            CreateMap<MessageAddVM, Message>();
            CreateMap<MessageAddVM, MessageVM>();
            CreateMap<MessageAddVM, MessageUpdateVM>();

            CreateMap<MessageUpdateVM, Message>();
            CreateMap<MessageUpdateVM, MessageVM>();
            CreateMap<MessageUpdateVM, MessageAddVM>();
            #endregion

            #region User & Auth
            CreateMap<User, UserVM>();
            CreateMap<User, UserRegisterVM>();
            CreateMap<User, UserUpdateVM>();
            CreateMap<User, UserAuthenticationVM>();

            CreateMap<UserVM, User>();
            CreateMap<UserVM, UserRegisterVM>();
            CreateMap<UserVM, UserUpdateVM>();
            CreateMap<UserVM, UserAuthenticationVM>();

            CreateMap<UserRegisterVM, User>();
            CreateMap<UserRegisterVM, UserVM>();
            CreateMap<UserRegisterVM, UserUpdateVM>();
            CreateMap<UserRegisterVM, UserAuthenticationVM>();

            CreateMap<UserUpdateVM, User>();
            CreateMap<UserUpdateVM, UserVM>();
            CreateMap<UserUpdateVM, UserRegisterVM>();
            CreateMap<UserUpdateVM, UserAuthenticationVM>();

            CreateMap<UserAuthenticationVM, User>();
            CreateMap<UserAuthenticationVM, UserVM>();
            CreateMap<UserAuthenticationVM, UserRegisterVM>();
            CreateMap<UserAuthenticationVM, UserUpdateVM>();
            #endregion
        }
    }
}
