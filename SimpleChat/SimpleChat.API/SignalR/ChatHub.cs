using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.SignalR;
using NGA.Data;
using NGA.Data.Service;
using NGA.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NGA.MonolithAPI.SignalR
{
    [EnableCors("CorsPolicy")]
    [AllowAnonymous]
    public class ChatHub : Hub
    {
        internal static List<Connection> Connections = new List<Connection>();

        private NGADbContext _con;
        private IMessageService _service;
        private readonly IMapper _mapper;
        public ChatHub(IMessageService service, IMapper mapper, NGADbContext con)
        {
            _service = service;
            _mapper = mapper;
            _con = con;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            //var groupId = httpContext.Request.Query["GroupId"];
            var userIdStr = httpContext.Request.Query["UserId"].FirstOrDefault();
            Guid userId = new Guid(userIdStr);

            var user = _con.Users.SingleOrDefault(s => s.Id == userId);

            if (user == null)
                return;

            Connection connection = new Connection();
            connection.ConnectionID = Context.ConnectionId;
            connection.Connected = true;
            connection.UserId = userId;
            //connection.GroupId = new Guid(groupId);

            Connections.Add(connection);

            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var connection = Connections.Find(a => a.ConnectionID == Context.ConnectionId);
            if (connection != null)
                Connections.Remove(connection);

            return base.OnDisconnectedAsync(exception);
        }


        public async Task AddToGroup(Guid groupId)
        {
            var connection = Connections.Find(a => a.ConnectionID == Context.ConnectionId);
            if (connection != null)
            {
                Connections.Remove(connection);

                if (_con.Groups.Any(a => (a.Id == groupId && !a.IsPrivate)
                || _con.GroupUsers.Any(x => x.GroupId == groupId && x.UserId == connection.UserId)))
                {
                    connection.GroupId = groupId;
                    Connections.Add(connection);

                    await Groups.AddToGroupAsync(connection.ConnectionID, connection.GroupId.ToString());
                }
            }
        }

        public async Task RemoveFromGroup(Guid groupId)
        {
            var connection = Connections.Find(a => a.ConnectionID == Context.ConnectionId);
            if (connection != null)
            {
                Connections.Remove(connection);
                connection.GroupId = null;
                Connections.Add(connection);

                await Groups.RemoveFromGroupAsync(connection.ConnectionID, connection.GroupId.ToString());
            }
        }

        public async Task SendMessage(MessageVM message)
        {
            var connection = Connections.Find(a => a.ConnectionID == Context.ConnectionId);
            if (connection != null && connection.GroupId == message.GroupId)
            {
                var model = _mapper.Map<MessageAddVM>(message);
                await _service.Add(model, message.UserId);

                await Clients.Group(message.GroupId.ToString()).SendAsync("ReceiveMessage", message);
            }
        }

        public async Task SendPrivateMessage(string message, Guid userId)
        {
            var connection = Connections.Find(a => a.UserId == userId);
            if (connection != null)
            {
                await Clients.User(connection.UserId.ToString()).SendAsync("ReceiveMessage", message);
            }
        }
    }


    public interface IChatHub
    {
        Task SendMessage();
        List<MessageVM> GetHistory(Guid userId);
    }


}
