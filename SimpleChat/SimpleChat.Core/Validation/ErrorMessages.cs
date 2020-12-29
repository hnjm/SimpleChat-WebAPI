using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleChat.Core.Validation
{
    public static class ErrorMessages
    {
        public static string ErrorThrew = "EE001";

        public static string MinStringLenght = "EF001";
        public static string MaxStringLenght = "EF002";

        public static string RecordNotFound = "Record couldn't found!";

        public static string NotOwnerOfRecord = "Not owner of the record!";

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
