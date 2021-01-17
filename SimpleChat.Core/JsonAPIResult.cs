using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SimpleChat.Core
{
    public class JsonAPIResult : JsonResult
    {
        public JsonAPIResult(object value, int statusCode)
            : base(value)
        {
            StatusCode = statusCode;
        }
    }
}
