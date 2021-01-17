using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleChat.Core.Redis
{
    public class RedisConfiguration
    {
        public string Connection { get; set; }
        public bool AllowAdmin { get; set; }
        public bool AbortOnConnectFail { get; set; }
        public int ConnectRetry { get; set; }
        public int ConnectTimeout { get; set; }
        public int KeepAlive { get; set; }
        public int SyncTimeout { get; set; }
        public string ClientName { get; set; }
        public string Password { get; set; }
        public bool ResolveDns { get; set; }
        public string TieBreaker { get; set; }
    }
}
