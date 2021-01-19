using System;
using System.Collections.Generic;
using StackExchange.Redis;
using SimpleChat.Core.Validation;

namespace SimpleChat.Core.Redis
{
    public static class RedisKeyFormat
    {
        public static string SignalRConnectionKeyFormat = "{hubName}_connection_{id}";
        public static string SignalRGroupKeyFormat = "{hubName}_group_{id}";

        public static string GetKey(Dictionary<string, string> parameters, string keyFormat)
        {
            if (keyFormat.IsNullOrEmptyString() || parameters.Count <= 0)
                return "";

            foreach (var item in parameters)
            {
                keyFormat = keyFormat.Replace("{" + item.Key + "}", item.Value);
            }

            return keyFormat;
        }
    }
}
