using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.SignalR;
using SimpleChat.API.Config;
using SimpleChat.Data;
using SimpleChat.Data.Service;
using SimpleChat.ViewModel.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleChat.API.SignalR
{
    public interface IChatHub
    {
        Task SendMessage(MessageVM message);
    }

    [EnableCors(ConstantValues.DefaultCorsPolicy)]
    [Authorize]
    public class ChatHub : Hub
    {
        // internal static List<Connection> Connections = new List<Connection>();

        private SimpleChatDbContext _con;
        private IMessageService _service;
        private readonly IMapper _mapper;

        public ChatHub(IMessageService service, IMapper mapper, SimpleChatDbContext con)
        {
            _service = service;
            _mapper = mapper;
            _con = con;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        public async Task AddToGroup(Guid groupId)
        {
            await Groups.AddToGroupAsync(this.Context.ConnectionId, groupId.ToString());
        }

        public async Task RemoveFromGroup(Guid groupId)
        {
            await Groups.RemoveFromGroupAsync(this.Context.ConnectionId, groupId.ToString());
        }

        public async Task SendMessage(MessageVM message)
        {
                var model = _mapper.Map<MessageAddVM>(message);
                await _service.AddAsync(model, message.CreateBy);

            string groupName = message.ChatRoomId.ToString();
            await Clients.Group(groupName).SendAsync("ReceiveMessage", message);
        }

        public async Task UserConnected()
        {
            await Clients.All.SendAsync("ReceiveUserConnection", Context.ConnectionId);
        }

        public async Task CheckHub()
        {
            await Clients.Caller.SendAsync("TestConnection", Context.ConnectionId);
        }
    }
}
