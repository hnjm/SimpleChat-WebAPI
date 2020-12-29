using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SimpleChat.Core.ViewModel;

namespace SimpleChat.Core.Helper
{
    public static class APIResult
    {
        public static APIResultVM CreateVM(bool isSuccessful = false, Guid? recId = null, List<string> messages = null)
        {
            if (messages == null)
                messages = new List<string>();

            var vm = new APIResultVM()
            {
                IsSuccessful = isSuccessful,
                RecId = recId,
                Messages = messages
            };
            return vm;
        }

        public static APIResultVM CreateVMWithRec<T>(T rec, bool isSuccessful = false, Guid? recId = null, List<string> messages = null)
        {
            if (messages == null)
                messages = new List<string>();

            var vm = new APIResultVM()
            {
                IsSuccessful = isSuccessful,
                RecId = recId,
                Rec = rec,
                Messages = messages
            };

            return vm;
        }
    }
}
