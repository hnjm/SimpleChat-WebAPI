using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleChat.Core.Validation
{
    public static class ValidationExtentionMethods
    {
        public static bool IsNull(this object value)
        {
            return value == null;
        }

        public static bool IsNullOrEmpty(this Guid? value)
        {
            return value == null || value == Guid.Empty;
        }

        public static bool IsNullOrEmpty(this string value)
        {
            return String.IsNullOrEmpty(value) || String.IsNullOrWhiteSpace(value);
        }

        public static bool IsNullOrEmpty(this int? value)
        {
            return value == null || value == 0;
        }

        public static bool IsNullOrEmpty(this DateTime? value)
        {
            return value == null || DateTime.MinValue == value;
        }
    }
}
