using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SimpleChat.Core.Validation
{
    public class GuidValidationAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "'{0}' does not contain a valid GUID";

        public GuidValidationAttribute()
            : base(DefaultErrorMessage)
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                if (value == null || new Guid(value.ToString()) == Guid.Empty)
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }
            catch (Exception)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            return null;
        }
    }
}
