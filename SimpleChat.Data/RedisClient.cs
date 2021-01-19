using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Sentry;
using SimpleChat.Core.ViewModel;
using StackExchange.Redis;
using SimpleChat.Core.Validation;
using SimpleChat.Core.Helper;

namespace SimpleChat.Data
{
    public sealed class RedisClient<TEntity, TKey>
        where TEntity : IBaseVM<TKey>
    {
        private readonly IDatabase _database;
        private readonly RedisDbContext _dbContext;
        private readonly APIResult<TKey> _apiResult;

        public RedisClient(RedisDbContext dbContext, APIResult<TKey> apiResult)
        {
            _database = dbContext.GetDatabase();
            _dbContext = dbContext;
            _apiResult = apiResult;
        }

        public IEnumerable<TEntity> GetAll(string searchPattern = "")
        {
            try
            {
                var keys = new List<RedisKey>();
                if (searchPattern.IsNullOrEmptyString())
                    keys = _dbContext.GetAllKeys().ToList();
                else
                    keys = _dbContext.GetAllKeys(searchPattern).ToList();

                var resultBytesArray = _database.StringGet(keys.ToArray());

                var results = new List<TEntity>();
                foreach (var resultBytes in resultBytesArray)
                {
                    if (resultBytes != RedisValue.Null)
                    {
                        var result = JsonSerializer.Deserialize<TEntity>(resultBytes);
                        results.Add(result);
                    }
                }

                return results;
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
                return null;
            }
        }

        public TEntity GetById(TKey key)
        {
            try
            {
                if (key == null)
                    return default(TEntity);

                string keyStr = System.Convert.ToString(key);
                var value = _database.StringGet(keyStr);

                if (!value.HasValue)
                    return default(TEntity);

                return JsonSerializer.Deserialize<TEntity>(value);
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
                return default(TEntity);
            }
        }

        public IAPIResultVM<TKey> Insert(TEntity entity, TimeSpan? expiryTime = null)
        {
            try
            {
				if (entity.Id.IsNull())
                    assignEntityId(ref entity);

                var keyStr = System.Convert.ToString(entity.Id);

                string json = JsonSerializer.Serialize(entity);
                bool isSuccessful = _database.StringSet(keyStr, json, expiryTime);

                return _apiResult.CreateVMWithRec<TEntity>(entity, entity.Id, isSuccessful);
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
                return _apiResult.CreateVM(entity.Id);
            }
        }

        public IAPIResultVM<TKey> Delete(TKey key)
        {
            try
            {
                string keyStr = System.Convert.ToString(key);
                bool isSuccessful = _database.KeyDelete(keyStr);
				return _apiResult.CreateVM(key, isSuccessful);
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
                return _apiResult.CreateVM(key);
            }
        }

		public string GetSetKey()
		{
			return "ids:" + typeof(TEntity).Name;
		}

		public long GetTypedSetLength()
		{
			string setKey = GetSetKey();
			var length = _database.SetLength(setKey);
			return length;
        }

		public long GetIncrementedKeyValue()
		{
			var length = GetTypedSetLength();
			length += 1;
			return length;
        }

		private void assignEntityId(ref TEntity entity)
		{
			long length;
			Type type = entity.GetType();
			var info = type.GetProperty("Id");
			if (typeof(TKey) == typeof(int))
			{
				length = GetIncrementedKeyValue();
				int intLength = System.Convert.ToInt32(length);
				info.SetValue(entity, length);
			}
			else if (typeof(TKey) == typeof(long))
			{
				length = GetIncrementedKeyValue();
				info.SetValue(entity, length);
			}
			else if (typeof(TKey) == typeof(Guid))
			{
				info.SetValue(entity, Guid.NewGuid());
			}
			else if (typeof(TKey) == typeof(String))
			{
				info.SetValue(entity, Guid.NewGuid().ToString());
			}
		}
    }
}
