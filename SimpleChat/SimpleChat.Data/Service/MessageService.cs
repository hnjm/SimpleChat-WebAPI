﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.Extensions.Logging;
using SimpleChat.Data.SubStructure;
using SimpleChat.Data.ViewModel.Message;
using SimpleChat.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleChat.Data.Service
{
    public class MessageService : BaseService<MessageAddVM, MessageUpdateVM, MessageVM, Message>, IMessageService
    {
        #region Ctor

        public MessageService(UnitOfWork _uow, IMapper _mapper, ILogger<IRepository<Message>> _repositoryLogger)
            : base(_uow, _mapper, _repositoryLogger)
        {

        }

        #endregion

        #region Methods                
        public List<MessageVM> GetMessagesByChatRoomId(Guid ChatRoomId)
        {
            var result = this.Repository.Query().Where(a => a.ChatRoomId == ChatRoomId).Select(a => new MessageVM()
            {
                Id = a.Id,
                Text = a.Text,
                ChatRoomId = a.ChatRoomId,
                CreateDT = a.CreateDT,
                CreateBy = a.CreateBy
            }).ToList();

            return result;
        }
        #endregion
    }

    public interface IMessageService : IBaseService<MessageAddVM, MessageUpdateVM, MessageVM, Message>
    {
        List<MessageVM> GetMessagesByChatRoomId(Guid ChatRoomId);
    }
}
