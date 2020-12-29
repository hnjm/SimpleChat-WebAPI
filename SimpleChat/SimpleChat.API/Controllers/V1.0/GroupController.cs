using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NGA.Core;
using NGA.Core.Helper;
using NGA.Data;
using NGA.Data.Service;
using NGA.Data.ViewModel;
using NGA.Domain;

namespace NGA.MonolithAPI.Controllers.V1
{
    //[ApiVersion("1.0")]
    //public class GroupController : DefaultApiCRUDController<GroupAddVM, GroupUpdateVM, GroupVM, IGroupService>
    //{
    //    public GroupController(IGroupService service)
    //         : base(service)
    //    {

    //    }

    //    [HttpGet]
    //    public virtual JsonResult GetByUserId(Guid userId)
    //    {
    //        try
    //        {
    //            var result = _service.GetByUserId(userId);

    //            if (result == null)
    //                return new JsonResult(APIResult.CreateVM(false, null, AppStatusCode.WRG01001));

    //            return new JsonResult(result);
    //        }
    //        catch (Exception ex)
    //        {
    //            return new JsonResult(APIResult.CreateVMWithError(ex, APIResult.CreateVM(false, null, AppStatusCode.ERR01001)));
    //        }
    //    }

    //    [HttpGet]
    //    public virtual JsonResult GetUsers(Guid groupId)
    //    {
    //        try
    //        {
    //            var result = _service.GetUsers(groupId);

    //            if (result == null)
    //                return new JsonResult(APIResult.CreateVM(false, null, AppStatusCode.WRG01001));

    //            return new JsonResult(result);
    //        }
    //        catch (Exception ex)
    //        {
    //            return new JsonResult(APIResult.CreateVMWithError(ex, APIResult.CreateVM(false, null, AppStatusCode.ERR01001)));
    //        }
    //    }
    //}
}