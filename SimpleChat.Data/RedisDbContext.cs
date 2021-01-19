using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Sentry;
using SimpleChat.Core.Redis;
using SimpleChat.Core.Validation;
using StackExchange.Redis;

namespace SimpleChat.Data
{
    public sealed class RedisDbContext
    {
        private RedisConfiguration _redisConfiguration;
        private ConnectionMultiplexer _connectionMultiplexer;

        private IConfiguration _configuration;

        public RedisDbContext(IConfiguration configuration)
        {
            _configuration = configuration;

            SetRedisConfiguration();
            SetConnectionMultiplexer();
        }

        public IDatabase GetDatabase(int index = 0)
		{
            try
            {
                return _connectionMultiplexer.GetDatabase(index);
            }
            catch (System.Exception e)
            {
                SentrySdk.CaptureException(e);
                throw e;
            }
        }

        public IEnumerable<RedisKey> GetAllKeys(string searchPattern = "")
        {
            if(searchPattern.IsNullOrEmptyString())
                return _connectionMultiplexer.GetServer(_redisConfiguration.Connection).Keys();
            else
                return _connectionMultiplexer.GetServer(_redisConfiguration.Connection).Keys(pattern: searchPattern);
        }

        private void SetConnectionMultiplexer()
        {
            try
            {
                ConfigurationOptions options = _redisConfiguration.ToRedisConfigurationOptions();
                options.EndPoints.Add(_redisConfiguration.Connection);
                _connectionMultiplexer = ConnectionMultiplexer.Connect(options);
            }
            catch (System.Exception e)
            {
                SentrySdk.CaptureException(e);
                throw e;
            }
        }

        private void SetRedisConfiguration()
        {
            try
            {
                _redisConfiguration = new RedisConfiguration();
                _configuration.Bind("Redis", _redisConfiguration);
            }
            catch (System.Exception e)
            {
                SentrySdk.CaptureException(e);
                throw e;
            }
        }
    }
}
