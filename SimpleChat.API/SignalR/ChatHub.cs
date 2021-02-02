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
using SimpleChat.Core.Serializer;
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
        Task JoinToGroup(string groupId);
        Task RemoveFromGroup(string groupId);
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
            var userIdStr = Context.GetHttpContext().Request.Query["userId"].FirstOrDefault();
            Guid.TryParse(userIdStr, out Guid userId);
            if (userId.IsEmptyGuid())
            {
                this.Context.Abort();
                return;
            }

            //if the user exist on the cache, remove the user first
            var connections = _connectionCache.GetAll(searchPattern: "ChatHub_connection_*").ToList();
            var connectionOfCurrentUser = connections.Where(a => a.UserId == userId).FirstOrDefault();
            if (connectionOfCurrentUser != null)
            {
                if (!connectionOfCurrentUser.GroupId.IsNullOrEmptyGuid())
                    RemoveUserFromGroup(connectionOfCurrentUser.GroupId.Value, ref connectionOfCurrentUser);

                DeleteConnectionFromCache(connectionOfCurrentUser.Id);
            }

            var connectionCachingResult = _connectionCache.Insert(new SignalRConnection()
            {
                Id = GetKeyForConnection(),
                UserId = userId,
                GroupId = null
            });
            if (!connectionCachingResult.IsSuccessful)
            {
                var e = new Exception(APIStatusCode.ERR04002);
                SentrySdk.CaptureException(e);
                this.Context.Abort();
                return;
            }

            await Clients.All.SendAsync("OnConnect", Newtonsoft.Json.JsonConvert.SerializeObject(new OnConnectVM()
            {
                UserId = userId
            }, new Newtonsoft.Json.JsonSerializerSettings()
            {
                ContractResolver = new LowercaseContractResolver()
            }));

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connection = Connection;
            if (!connection.Equals(default(SignalRConnection)))
            {
                DeleteConnectionFromCache(GetKeyForConnection());

                if (!connection.GroupId.IsNullOrEmptyGuid())
                {
                    var group = _groupCache.GetById(GetKeyForGroup(connection.GroupId.Value));
                    group.ConnectedUsers.Remove(connection.UserId);

                    var groupUpdateResult = _groupCache.Insert(group);
                    if (!groupUpdateResult.IsSuccessful)
                        SentrySdk.CaptureException(new Exception(APIStatusCode.ERR04005));
                }
            }

            await Clients.All.SendAsync("OnDisconnect", Newtonsoft.Json.JsonConvert.SerializeObject(new OnDisconnectVM()
            {
                UserId = connection.UserId
            }, new Newtonsoft.Json.JsonSerializerSettings()
            {
                ContractResolver = new LowercaseContractResolver()
            }));

            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinToGroup(string groupId)
        {
            Guid.TryParse(groupId, out Guid groupIdGuid);
            var connection = Connection;
            if (!connection.Equals(default(SignalRConnection)))
            {
                //Remove the connection from existing group
                if (!connection.GroupId.IsNullOrEmptyGuid())
                {
                    await RemoveFromGroup(connection.GroupId.ToString());
                }

                //Check is the Chat Room exist on database
                if (!IsGroupExistOnDatabase(groupIdGuid))
                    return;

                var group = _groupCache.GetById(GetKeyForGroup(groupIdGuid));
                if (group == null || group.Equals(default(SignalRGroup)))
                {
                    group = new SignalRGroup();
                    group.Id = GetKeyForGroup(groupIdGuid);
                    group.ConnectedUsers = new List<Guid>() { connection.UserId };
                }
                else
                {
                    group.ConnectedUsers.Add(connection.UserId);
                }

                //Update connection groupId field
                connection.GroupId = groupIdGuid;

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
                await Clients.Group(groupIdGuid.ToString()).SendAsync("OnJoinToGroup", Newtonsoft.Json.JsonConvert.SerializeObject(new OnJoinToGroupVM()
                {
                    GroupId = groupIdGuid,
                    UserId = connection.UserId
                }, new Newtonsoft.Json.JsonSerializerSettings()
                {
                    ContractResolver = new LowercaseContractResolver()
                }));

                await Groups.AddToGroupAsync(Context.ConnectionId, groupIdGuid.ToString());
            }
        }

        public async Task RemoveFromGroup(string groupId)
        {
            Guid.TryParse(groupId, out Guid groupIdGuid);
            var connection = Connection;
            if (!connection.Equals(default(SignalRConnection)))
            {
                //Check is the Chat Room exist on database
                if (!IsGroupExistOnDatabase(groupIdGuid))
                    return;

                var result = RemoveUserFromGroup(groupIdGuid, ref connection);
                if (!result)
                    return;

                //Send notification to members of group
                await Clients.Group(groupIdGuid.ToString()).SendAsync("OnLeaveFromGroup", Newtonsoft.Json.JsonConvert.SerializeObject(new OnLeaveFromGroupVM()
                {
                    GroupId = groupIdGuid,
                    UserId = connection.UserId
                }, new Newtonsoft.Json.JsonSerializerSettings()
                {
                    ContractResolver = new LowercaseContractResolver()
                }));

                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupIdGuid.ToString());
            }
        }

        public async Task SendMessage(string text)
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
                    Text = text
                }, connection.UserId);
                if (dbResult.ResultIsNotTrue())
                {
                    SentrySdk.CaptureException(new Exception(APIStatusCode.ERR04008));
                    return;
                }
                var result = new OnReceivedMessageVM()
                {
                    GroupId = connection.GroupId.Value,
                    SenderId = connection.UserId,
                    Text = text
                };
                await Clients.Group(connection.GroupId.Value.ToString()).SendAsync("ReceiveMessage",
                Newtonsoft.Json.JsonConvert.SerializeObject(result, new Newtonsoft.Json.JsonSerializerSettings()
                {
                    ContractResolver = new LowercaseContractResolver()
                }));
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
                    ActiveUsersResponseVM result = new ActiveUsersResponseVM()
                    {
                        ActiveUsers = connections.ToList()
                    };

                    await Clients.Caller.SendAsync("ReceiveActiveUsers", Newtonsoft.Json.JsonConvert.SerializeObject(result, new Newtonsoft.Json.JsonSerializerSettings()
                    {
                        ContractResolver = new LowercaseContractResolver()
                    }));
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

                ActiveUsersOfGroupResponseVM result = new ActiveUsersOfGroupResponseVM()
                {
                    ActiveUsers = group.ConnectedUsers,
                    GroupId = RedisKeyFormat.GetIdFromKey(group.Id, RedisKeyFormat.SignalRKeySeperator, 2)
                };

                await Clients.Caller.SendAsync("ReceiveActiveUsersOfGroup", Newtonsoft.Json.JsonConvert.SerializeObject(result, new Newtonsoft.Json.JsonSerializerSettings()
                {
                    ContractResolver = new LowercaseContractResolver()
                }));
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

        private void DeleteConnectionFromCache(string key)
        {
            var deleteResult = _connectionCache.Delete(key);
            if (!deleteResult.IsSuccessful)
            {
                var e = new Exception(APIStatusCode.ERR04004);
                SentrySdk.CaptureException(e);
            }
        }

        private bool RemoveUserFromGroup(Guid groupId, ref SignalRConnection connection)
        {
            var group = _groupCache.GetById(GetKeyForGroup(groupId));
            if (group.Equals(default(SignalRGroup)))
            {
                SentrySdk.CaptureException(new Exception(APIStatusCode.ERR04007));
                return false;
            }

            group.ConnectedUsers.Remove(connection.UserId);
            connection.GroupId = null;

            //update connection on cache
            var connectionUpdateResult = _connectionCache.Insert(connection);
            if (!connectionUpdateResult.IsSuccessful)
            {
                SentrySdk.CaptureException(new Exception(APIStatusCode.ERR04002));
                return false;
            }

            //update group on cache
            var groupUpdateResult = _groupCache.Insert(group);
            if (!groupUpdateResult.IsSuccessful)
            {
                SentrySdk.CaptureException(new Exception(APIStatusCode.ERR04003));
                return false;
            }

            return true;
        }

        #endregion
    }
}
