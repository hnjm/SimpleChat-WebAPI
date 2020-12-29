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

        public static bool IsEmptyGuid(this Guid value)
        {
            return value == null || value == Guid.Empty;
        }

        public static bool IsNullOrEmptyGuid(this Guid? value)
        {
            return value == null || value == Guid.Empty;
        }

        public static bool IsNullOrEmptyString(this string value)
        {
            return String.IsNullOrEmpty(value) || String.IsNullOrWhiteSpace(value);
        }

        public static bool IsNullOrEmptyInt(this int? value)
        {
            return value == null || value == 0;
        }

        public static bool IsNullOrEmptyDateTime(this DateTime? value)
        {
            return value == null || DateTime.MinValue == value;
        }
    }
}
