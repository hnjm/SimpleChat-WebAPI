using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using SimpleChat.Core.Validation;

namespace SimpleChat.Core.Serializer
{
    public class LowercaseContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            if (propertyName.IsNullOrEmptyString())
                return "";
            string firstChar = propertyName.Substring(0, 1);

            return firstChar.ToLower() + propertyName.Substring(1, propertyName.Length - 1);
        }
    }
}
