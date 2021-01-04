﻿using AutoMapper;
using SimpleChat.Data.ViewModel;
using SimpleChat.Data.ViewModel.ChatRoom;
using SimpleChat.Data.ViewModel.Message;
using SimpleChat.Data.ViewModel.User;
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
            //CreateMap<Suggestion, SuggestionVM>()
            //    .ForMember(m => m.CategoryName, o => o.MapFrom(s => s.Category.Name))
            //    .ForMember(m => m.CreateById, o => o.MapFrom(s => s.CreateBy))
            //    .ForMember(m => m.CreateDateTime, o => o.MapFrom(s => s.CreateDT))
            //    .ForMember(m => m.TotalReaction, o => o.MapFrom(s => s.DislikeAmount + s.LikeAmount + ((s.SuggestionComments != null ? s.SuggestionComments.Count : 0) * 2)))
            //    .ForMember(m => m.CommentCount, o => o.MapFrom(s => s.SuggestionComments != null ? s.SuggestionComments.Count : 0));

            //CreateMap<SuggestionVM, Suggestion>()
            //    .ForMember(m => m.CreateBy, o => o.Ignore())
            //    .ForMember(m => m.LikeAmount, o => o.Ignore())
            //    .ForMember(m => m.DislikeAmount, o => o.Ignore());

            //CreateMap<SuggestionVM, SuggestionSaveVM>()
            //    .ForMember(m => m.CreateBy, o => o.MapFrom(s => s.CreateById));

            //CreateMap<SuggestionSaveVM, Suggestion>()
            //    .ForMember(m => m.CreateBy, o => o.Ignore())
            //    .ForMember(m => m.CreateDT, o => o.Ignore());
            //CreateMap<Suggestion, SuggestionSaveVM>();


            //CreateMap<SuggestionComment, SuggestionCommentVM>();
            //// .ForMember(m => m.CreateBy, o => o.MapFrom(s => s.CreateBy));
            //CreateMap<SuggestionCommentVM, SuggestionCommentSaveVM>().ReverseMap();
            //CreateMap<SuggestionComment, SuggestionCommentSaveVM>().ReverseMap();

            //CreateMap<SuggestionReaction, SuggestionReactionVM>().ReverseMap();
            //CreateMap<SuggestionReactionVM, SuggestionReactionSaveVM>().ReverseMap();
            //CreateMap<SuggestionReaction, SuggestionReactionSaveVM>().ReverseMap();

            //CreateMap<Category, CategoryVM>()
            //    .ForMember(m => m.CreateById, o => o.MapFrom(s => s.CreateBy))
            //    .ForMember(m => m.CreateDateTime, o => o.MapFrom(s => s.CreateDT));

            //CreateMap<CategorySaveVM, Category>()
            //    .ForMember(m => m.CreateBy, o => o.Ignore())
            //    .ForMember(m => m.CreateDT, o => o.Ignore());
            //CreateMap<Category, CategorySaveVM>();
            //CreateMap<CategoryVM, CategorySaveVM>().ReverseMap();

            //CreateMap<User, ProfileVM>();


            CreateMap<ChatRoom, ChatRoomVM>().ReverseMap();
            CreateMap<ChatRoom, ChatRoomAddVM>().ReverseMap();
            CreateMap<ChatRoom, ChatRoomUpdateVM>().ReverseMap();

            CreateMap<ChatRoomUser, ChatRoomUserVM>().ReverseMap();
            CreateMap<ChatRoomUser, ChatRoomUserAddVM>().ReverseMap();
            CreateMap<ChatRoomUser, ChatRoomUserUpdateVM>().ReverseMap();

            CreateMap<Message, MessageVM>().ReverseMap();
            CreateMap<Message, MessageAddVM>().ReverseMap();
            CreateMap<Message, MessageUpdateVM>().ReverseMap();

            CreateMap<UserRegisterVM, User>();
            CreateMap<User, UserAuthenticationVM>();
            // CreateMap<Message, MessageUpdateVM>().ReverseMap();
            // CreateMap<Message, MessageUpdateVM>().ReverseMap();
            // CreateMap<Message, MessageUpdateVM>().ReverseMap();
        }
    }
}
