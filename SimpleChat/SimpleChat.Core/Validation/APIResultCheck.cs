using SimpleChat.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleChat.Core.Validation
{
    public static class APIResultCheck
    {
        public static bool ResultIsTrue(this IResultVM value)
        {
            if (value.IsNull())
                return false;

            return value.IsSuccessful;
        }

        public static bool ResultIsNotTrue(this IResultVM value)
        {
            return !ResultIsTrue(value);
        }
    }
}
