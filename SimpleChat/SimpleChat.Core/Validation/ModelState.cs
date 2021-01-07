using Microsoft.AspNetCore.Mvc.ModelBinding;
using SimpleChat.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleChat.Core.Validation
{
    public static class ModelState
    {
        public static IEnumerable<APIResultErrorCodeVM> GetErrors(this ModelStateDictionary modelStateDictionary)
        {
            return from sd in modelStateDictionary.Values
                         from e in sd.Errors
                         select new APIResultErrorCodeVM()
                         {
                             Field = sd.GetType().Name,
                             ErrorCode = e.ErrorMessage
                         };
        }
    }
}
