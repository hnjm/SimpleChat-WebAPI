using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleChat.Core.Helper
{
    public static class Convert
    {
        public static bool TryParseEnum<TEnum>(this int enumValue, out TEnum retVal)
        {
            retVal = default(TEnum);
            bool success = System.Enum.IsDefined(typeof(TEnum), enumValue);
            if (success)
            {
                retVal = (TEnum)System.Enum.ToObject(typeof(TEnum), enumValue);
            }
            return success;
        }

        public static bool ToBoolean(this string value)
        {
            switch (value.ToLower())
            {
                case "true":
                    return true;
                case "t":
                    return true;
                case "1":
                    return true;
                case "0":
                    return false;
                case "false":
                    return false;
                case "f":
                    return false;
                default:
                    return false;
            }
        }

        internal static byte ToByte<T>(T value) where T : new()
        {
            try
            {
                return System.Convert.ToByte(value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static List<string> StrToList(string text, string seperator)
        {
            if (string.IsNullOrWhiteSpace(text))
                return new List<string>();

            return new List<string>(
                           text.Split(new string[] { seperator },
                           StringSplitOptions.RemoveEmptyEntries));

        }
    }
}
