using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SimpleChat.Core.ViewModel;

namespace SimpleChat.Core.Helper
{
    public static class APIResult
    {
        public static APIResultVM CreateVM(bool isSuccessful = false, Guid? recId = null, string statusCode = "")
        {
            var vm = new APIResultVM()
            {
                IsSuccessful = isSuccessful,
                RecId = recId,
                StatusCode = statusCode,
            };
            return vm;
        }

        public static APIResultVM CreateVMWithRec<T>(T rec, bool isSuccessful = false, Guid? recId = null, string statusCode = "")
        {
            var vm = new APIResultVM()
            {
                IsSuccessful = isSuccessful,
                RecId = recId,
                StatusCode = statusCode,
                Rec = rec,
            };

            return vm;
        }
    }
}
