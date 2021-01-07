using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleChat.Core.Validation
{
    public static class AttributeErrorMessages
    {
        public static string ErrorThrew = "EE001";

        public static string MinStringLength = "EF001";
        public static string MaxStringLength = "EF002";

        public static string CombineWithParams(string errorMessage, params string[] args)
        {
            if (args == null || args.Length == 0)
                return errorMessage;

            StringBuilder sb = new StringBuilder(errorMessage);
            sb.Append(":");

            for (int i = 0; i < args.Length; i++)
            {
                sb.Append(args[i]);

                if (i != args.Length - 1)
                    sb.Append(",");
            }

            return errorMessage;
        }
    }
}
