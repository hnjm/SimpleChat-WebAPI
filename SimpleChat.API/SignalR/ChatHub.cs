using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Sentry;
using SimpleChat.API.Config;
using SimpleChat.Core;
using SimpleChat.Core.Helper;
using SimpleChat.Core.Redis;
using SimpleChat.Core.Validation;
using SimpleChat.Data;
using SimpleChat.Data.Service;
using SimpleChat.Domain;
using SimpleChat.ViewModel.Message;
using SimpleChat.ViewModel.SignalR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SimpleChat.API.SignalR
{
    public interface IChatHub
    {
        Task OnConnectedAsync();
        Task OnDisconnectedAsync(Exception exception);
        Task AddToGroup(string groupIdStr);
        Task RemoveFromGroup(RemoveFromGroupVM data);
        Task SendMessage(string data);
        Task GetActiveUsers();
        Task GetActiveUsersOfGroup();
        Task CheckHub();
    }

    [EnableCors(ConstantValues.DefaultCorsPolicy)]
    [Authorize]
    public class ChatHub : Hub, IChatHub
    {
        private readonly SimpleChatDbContext _con;
        private readonly IMessageService _service;
        private readonly IMapper _mapper;
        private readonly RedisClient<SignalRConnection, string> _connectionCache;
        private readonly RedisClient<SignalRGroup, string> _groupCache;

        public ChatHub(IMessageService service,
            IMapper mapper,
            SimpleChatDbContext con,
            RedisClient<SignalRConnection, string> connectionCache,
            RedisClient<SignalRGroup, string> groupCache)
        {
            _service = service;
            _mapper = mapper;
            _con = con;
            _connectionCache = connectionCache;
            _groupCache = groupCache;
        }

        private SignalRConnection Connection
        {
            get
            {
                return _connectionCache.GetById(GetKeyForConnection());
            }
        }

        public override async Task OnConnectedAsync()
        {
            var userIdStr = Context.GetHttpContext().Request.Query["userId"].FirstOrDefault();//GetRequestBody<OnConnectVM>();
            Guid.TryParse(userIdStr, out Guid userId);
            if (userId.IsEmptyGuid())//data.Equals(default(OnConnectVM)))
            {
                await base.OnDisconnectedAsync(new Exception(APIStatusCode.ERR04001));
                return;
            }

            var connectionCachingResult = _connectionCache.Insert(new SignalRConnection()
            {
                Id = GetKeyForConnection(),
                UserId = userId, //data.UserId,
                GroupId = null
            });
            if (!connectionCachingResult.IsSuccessful)
            {
                var e = new Exception(APIStatusCode.ERR04002);
                SentrySdk.CaptureException(e);
                await base.OnDisconnectedAsync(e);
                return;
            }

            await Clients.All.SendAsync("OnConnect", new OnConnectVM() { UserId = userId });

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connection = Connection;
            if (!connection.Equals(default(SignalRConnection)))
            {
                var deleteResult = _connectionCache.Delete(GetKeyForConnection());
                if (!deleteResult.IsSuccessful)
                {
                    var e = new Exception(APIStatusCode.ERR04004);
                    SentrySdk.CaptureException(e);
                }

                if (!connection.GroupId.IsNullOrEmptyGuid())
                {
                    var group = _groupCache.GetById(GetKeyForGroup(connection.GroupId.Value));
                    group.ConnectedUsers.Remove(connection.UserId);

                    var groupUpdateResult = _groupCache.Insert(group);
                    if (!groupUpdateResult.IsSuccessful)
                        SentrySdk.CaptureException(new Exception(APIStatusCode.ERR04005));
                }
            }

            await Clients.All.SendAsync("OnDisconnect",  JsonSerializer.Serialize("Disconnected"));

            await base.OnDisconnectedAsync(exception);
        }

        public async Task AddToGroup(string groupIdStr)
        {
            Guid.TryParse(groupIdStr, out Guid groupId);
            var connection = Connection;
            if (!connection.Equals(default(SignalRConnection)))
            {
                //Remove the connection from existing group
                if (!connection.GroupId.IsNullOrEmptyGuid())
                {
                    await RemoveFromGroup(new RemoveFromGroupVM()
                    {
                        GroupId = connection.GroupId.Value
                    });
                }

                //Check is the Chat Room exist on database
                if (!IsGroupExistOnDatabase(groupId))
                    return;

                var group = _groupCache.GetById(GetKeyForGroup(groupId));
                if (group == null || group.Equals(default(SignalRGroup)))
                {
                    group = new SignalRGroup();
                    group.Id = GetKeyForGroup(groupId);
                    group.ConnectedUsers = new List<Guid>() { connection.UserId };
                }
                else
                {
                    group.ConnectedUsers.Add(connection.UserId);
                }

                //Update connection groupId field
                connection.GroupId = groupId;

                //update connection on cache
                var connectionUpdateResult = _connectionCache.Insert(connection);
                if (!connectionUpdateResult.IsSuccessful)
                {
                    SentrySdk.CaptureException(new Exception(APIStatusCode.ERR04002));
                    return;
                }

                //update group on cache
                var groupUpdateResult = _groupCache.Insert(group);
                if (!groupUpdateResult.IsSuccessful)
                {
                    SentrySdk.CaptureException(new Exception(APIStatusCode.ERR04003));
                    return;
                }

                //send notification to members of group
                await Clients.Group(groupId.ToString()).SendAsync("OnJoinToGroup", connection.UserId.ToString());

                await Groups.AddToGroupAsync(Context.ConnectionId, groupId.ToString());
            }
        }

        public async Task RemoveFromGroup(RemoveFromGroupVM data)
        {
            var connection = Connection;
            if (!connection.Equals(default(SignalRConnection)))
            {
                //Check is the Chat Room exist on database
                if (!IsGroupExistOnDatabase(data.GroupId))
                    return;

                //Get cached group data
                var group = _groupCache.GetById(GetKeyForGroup(data.GroupId));
                if (group.Equals(default(SignalRGroup)))
                {
                    SentrySdk.CaptureException(new Exception(APIStatusCode.ERR04007));
                    return;
                }

                group.ConnectedUsers.Remove(connection.UserId);
                connection.GroupId = null;

                //update connection on cache
                var connectionUpdateResult = _connectionCache.Insert(connection);
                if (!connectionUpdateResult.IsSuccessful)
                {
                    SentrySdk.CaptureException(new Exception(APIStatusCode.ERR04002));
                    return;
                }

                //update group on cache
                var groupUpdateResult = _groupCache.Insert(group);
                if (!groupUpdateResult.IsSuccessful)
                {
                    SentrySdk.CaptureException(new Exception(APIStatusCode.ERR04003));
                    return;
                }

                //Send notification to members of group
                await Clients.Group(data.GroupId.ToString()).SendAsync("OnLeaveFromGroup", JsonSerializer.Serialize("Remove From Group"));

                await Groups.RemoveFromGroupAsync(Context.ConnectionId, data.GroupId.ToString());
            }
        }

        public async Task SendMessage(string data)
        {
            var connection = Connection;
            if (!connection.Equals(default(SignalRConnection)) && !connection.GroupId.IsNullOrEmptyGuid())
            {
                var group = _groupCache.GetById(GetKeyForGroup(connection.GroupId.Value));
                if (group.Equals(default(SignalRGroup)))
                {
                    SentrySdk.CaptureException(new Exception(APIStatusCode.ERR04007));
                    return;
                }

                var dbResult = await _service.AddAsync(new MessageAddVM()
                {
                    ChatRoomId = connection.GroupId.Value,
                    Text = data
                }, connection.UserId);
                if (dbResult.ResultIsNotTrue())
                {
                    SentrySdk.CaptureException(new Exception(APIStatusCode.ERR04008));
                    return;
                }
                var result = new OnReceivedMessageVM(){
                    GroupId = connection.GroupId.Value,
                    SenderId = connection.UserId,
                    Text = data
                };
                await Clients.Group(connection.GroupId.Value.ToString()).SendAsync("ReceiveMessage", JsonSerializer.Serialize(result));
            }
        }

        public async Task GetActiveUsers()
        {
            var connection = Connection;
            if (!connection.Equals(default(SignalRConnection)))
            {
                var connections = _connectionCache.GetAll(GetKeyForConnection(true));

                if (connections.Any())
                {
                    await Clients.Caller.SendAsync("ReceiveActiveUsers", JsonSerializer.Serialize(connections.ToList()));
                }
            }
        }

        public async Task GetActiveUsersOfGroup()
        {
            var connection = Connection;
            if (!connection.Equals(default(SignalRConnection)))
            {
                var group = _groupCache.GetById(GetKeyForGroup(connection.GroupId.Value));
                if (group.Equals(default(SignalRGroup)))
                {
                    SentrySdk.CaptureException(new Exception(APIStatusCode.ERR04007));
                    return;
                }

                await Clients.Caller.SendAsync("ReceiveActiveUsersOfGroup", group.ConnectedUsers);
            }
        }

        public async Task CheckHub()
        {
            await Clients.Caller.SendAsync("TestConnection", Context.ConnectionId);
        }

        #region Helper Methods

        private T GetRequestBody<T>()
        {
            try
            {
                var requestBody = Context.GetHttpContext().Request.ReadBodyAsString();
                var model = JsonSerializer.Deserialize<T>(requestBody);

                return model;
            }
            catch (ArgumentNullException e)
            {
                SentrySdk.CaptureException(e);
                return default(T);
            }
            catch (JsonException e)
            {
                SentrySdk.CaptureException(e);
                return default(T);
            }
            catch (NotSupportedException e)
            {
                SentrySdk.CaptureException(e);
                return default(T);
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
                return default(T);
            }
        }

        private string GetKeyForConnection(bool getKeyForAll = false)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("hubName", "ChatHub");
            if (!getKeyForAll)
                parameters.Add("id", Context.ConnectionId);
            else
                parameters.Add("id", "*");

            return RedisKeyFormat.GetKey(parameters, RedisKeyFormat.SignalRConnectionKeyFormat);
        }

        private string GetKeyForGroup(Guid? groupId)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("hubName", "ChatHub");
            if (!groupId.IsNullOrEmptyGuid())
                parameters.Add("id", groupId.Value.ToString());
            else
                parameters.Add("id", "*");

            return RedisKeyFormat.GetKey(parameters, RedisKeyFormat.SignalRGroupKeyFormat);
        }

        private bool IsGroupExistOnDatabase(Guid groupId)
        {
            bool isGroupExist = _con.Set<ChatRoom>().AsNoTracking().Any(a => a.Id == groupId);
            if (!isGroupExist)
            {
                SentrySdk.CaptureException(new Exception(APIStatusCode.ERR04006));
            }

            return isGroupExist;
        }

        #endregion
    }
}
