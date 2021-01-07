using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SimpleChat.Core.Validation
{
    public class MinLengthErrorMessageAttribute : ValidationAttribute
    {
        private readonly int minLength = 0;

        public MinLengthErrorMessageAttribute(int minLength)
        {
            this.minLength = minLength;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                int currentLength = 0;
                Type type = value.GetType();

                switch (type)
                {
                    case var _ when type.Equals(typeof(int)):
                        int.TryParse(value.ToString(), out currentLength);
                        break;
                    case var _ when type.Equals(typeof(string)):
                        string stringValue = Convert.ToString(value);
                        currentLength = stringValue.Length;
                        break;
                    default:
                        break;
                }

                if(currentLength < minLength && minLength > 0)
                {
                    string errorMessage = AttributeErrorMessages.CombineWithParams(AttributeErrorMessages.MinStringLength, minLength.ToString());

                    return new ValidationResult(errorMessage);
                }
            }
            catch (Exception)
            {
                return new ValidationResult(AttributeErrorMessages.ErrorThrew);
            }

            return null;
        }
    }
}
