
using StackExchange.Redis;

namespace SimpleChat.Core.Redis
{
    public static class RedisExtensions
    {
        public static string ToTypedKey(this string key, string type)
		{
			return type.ToLower() + ":" + key;
		}

        public static ConfigurationOptions ToRedisConfigurationOptions(this RedisConfiguration src)
        {
            ConfigurationOptions options = new ConfigurationOptions();
            if(src.AbortOnConnectFail)
            {
                options.AbortOnConnectFail = true;
            }
            if(src.AllowAdmin)
            {
                options.AllowAdmin = true;
            }
            if(!string.IsNullOrEmpty(src.ClientName))
            {
                options.ClientName = src.ClientName;
            }
            if(src.ConnectRetry!=0)
            {
                options.ConnectRetry = src.ConnectRetry;
            }
            if(src.ConnectTimeout!=0)
            {
                options.ConnectTimeout = src.ConnectTimeout;
            }
            if(src.KeepAlive!=0)
            {
                options.KeepAlive = src.KeepAlive;
            }
            if(!string.IsNullOrEmpty(src.Password))
            {
                options.Password = src.Password;
            }
            if(src.ResolveDns)
            {
                options.ResolveDns = true;
            }
            if(src.SyncTimeout!=0)
            {
                options.SyncTimeout = src.SyncTimeout;
            }
            if(!string.IsNullOrEmpty(src.TieBreaker))
            {
                options.TieBreaker = src.TieBreaker;
            }

            return options;
        }
    }
}
