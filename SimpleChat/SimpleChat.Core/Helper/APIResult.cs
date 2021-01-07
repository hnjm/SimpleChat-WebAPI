using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SimpleChat.Core.Validation;
using SimpleChat.Core.ViewModel;

namespace SimpleChat.Core.Helper
{
    public static class APIResult
    {
        public static APIResultVM CreateVM(
            bool isSuccessful = false,
            Guid? recId = null,
            IEnumerable<APIResultErrorCodeVM> errors = null)
        {
            var vm = new APIResultVM()
            {
                IsSuccessful = isSuccessful,
                RecId = recId,
                Errors = errors
            };
            return vm;
        }

        public static APIResultVM CreateVMWithRec<T>(
            T rec,
            bool isSuccessful = false,
            Guid? recId = null,
            IEnumerable<APIResultErrorCodeVM> errors = null)
        {
            var vm = new APIResultVM()
            {
                IsSuccessful = isSuccessful,
                RecId = recId,
                Rec = rec,
                Errors = errors
            };

            return vm;
        }
        public static APIResultVM CreateVMWithStatusCode(
            bool isSuccessful = false,
            Guid? recId = null,
            string statusCode = "")
        {
            var errors = new List<APIResultErrorCodeVM>();
            errors.Add(new APIResultErrorCodeVM()
            {
                Field = "General",
                ErrorCode = statusCode
            });

            return CreateVM(isSuccessful, recId, errors.AsEnumerable<APIResultErrorCodeVM>());
        }

        public static APIResultVM CreateVMWithIdentityErrors(
            bool isSuccessful = false,
            Guid? recId = null,
            IEnumerable<IdentityError> errors = null)
        {
            return CreateVM(
                isSuccessful,
                recId,
                errors.Select(x => new APIResultErrorCodeVM()
                {
                    ErrorCode = x.Description,
                    Field = x.Code
                }).AsEnumerable<APIResultErrorCodeVM>());
        }

        public static APIResultVM CreateVMWithModelState(
            bool isSuccessful = false,
            Guid? recId = null,
            ModelStateDictionary modelStateDictionary = null)
        {
            return CreateVM(
                isSuccessful,
                recId,
                modelStateDictionary.GetErrors());
        }

        public static APIResultVM CreateVMWithRecAndIdentityErrors<T>(
            T rec,
            bool isSuccessful = false,
            Guid? recId = null,
            IEnumerable<IdentityError> errors = null)
        {
            return CreateVMWithRec<T>(
                rec,
                isSuccessful,
                recId,
                errors.Select(x => new APIResultErrorCodeVM()
                {
                    ErrorCode = x.Description,
                    Field = x.Code
                }).AsEnumerable<APIResultErrorCodeVM>());
        }
    }
}
