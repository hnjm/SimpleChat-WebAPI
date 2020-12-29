using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NGA.Core;
using NGA.Core.Helper;
using NGA.Core.Model;
using NGA.Core.Validation;
using NGA.Data.SubStructure;

namespace NGA.MonolithAPI.Controllers.V1
{
    public abstract class DefaultApiCRUDController<A, U, G, S> : DefaultApiController
             where A : AddVM, IAddVM, new()
             where U : UpdateVM, IUpdateVM, new()
             where G : BaseVM, IBaseVM, new()
             where S : ICRUDService<A, U, G>
    {
        protected S _service;

        public DefaultApiCRUDController(S service)
        {
            this._service = service;
        }

        [HttpGet]
        public virtual JsonResult Get()
        {
            try
            {
                var result = _service.GetAll();

                if (result == null)
                    return new JsonResult(APIResult.CreateVM(false, null, AppStatusCode.WRG01001));

                return new JsonResult(new List<G>());
            }
            catch (Exception ex)
            {
                return new JsonResult(APIResult.CreateVMWithError(ex, APIResult.CreateVM(false, null, AppStatusCode.ERR01001)));
            }
        }

        [HttpGet]
        public virtual async Task<JsonResult> GetById(Guid id)
        {
            try
            {
                if (Validation.IsNullOrEmpty(id))
                    return new JsonResult(APIResult.CreateVM(false, null, AppStatusCode.WRG01002));

                var result = await _service.GetByIdAsync(id);

                if (result == null)
                    return new JsonResult(APIResult.CreateVM(false, null, AppStatusCode.WRG01001));

                return new JsonResult(APIResult.CreateVMWithRec<G>(result, true, result.Id));
            }
            catch (Exception ex)
            {
                return new JsonResult(APIResult.CreateVMWithError(ex, APIResult.CreateVM(false, null, AppStatusCode.ERR01001)));
            }
        }

        [HttpPost]
        public virtual async Task<JsonResult> Add(A model)
        {
            APIResultVM result = new APIResultVM();

            try
            {
                if (Validation.IsNull(model))
                    APIResult.CreateVM(false, null, AppStatusCode.WRG01001);

                result = await _service.Add(model);

                if (Validation.ResultIsNotTrue(result))
                    return new JsonResult(result);

                return new JsonResult(APIResult.CreateVM(true, result.RecId));
            }
            catch (Exception ex)
            {
                return new JsonResult(APIResult.CreateVMWithError(ex, APIResult.CreateVM(false, result.RecId)));
            }
        }

        [HttpPut]
        public virtual async Task<JsonResult> Update(Guid id, U model)
        {
            APIResultVM result = new APIResultVM();

            try
            {
                if (Validation.IsNull(model))
                    APIResult.CreateVM(false, id, AppStatusCode.WRG01001);

                result = await _service.Update(id, model);

                if (Validation.ResultIsNotTrue(result))
                    return new JsonResult(result);

                return new JsonResult(APIResult.CreateVM(true, result.RecId));
            }
            catch (Exception ex)
            {
                return new JsonResult(APIResult.CreateVMWithError(ex, APIResult.CreateVM(false, result.RecId)));
            }
        }

        [HttpDelete]
        public virtual async Task<JsonResult> Delete(Guid id)
        {
            APIResultVM result = new APIResultVM();

            try
            {
                if (id == null || id == Guid.Empty)
                    APIResult.CreateVM(false, null, AppStatusCode.WRG01001);

                result = await _service.Delete(id);

                if (Validation.ResultIsNotTrue(result))
                    return new JsonResult(result);

                return new JsonResult(APIResult.CreateVM(true, result.RecId));
            }
            catch (Exception ex)
            {
                return new JsonResult(APIResult.CreateVMWithError(ex, APIResult.CreateVM(false, result.RecId)));
            }
        }
    }

    public interface IBaseController<A, U>
        where A : AddVM, IAddVM
        where U : UpdateVM, IUpdateVM
    {
        Task<ActionResult> Add(A model, bool saveAndClose = true);
        Task<ActionResult> Delete(Guid id);
        ActionResult Get(Guid id);
        Task<ActionResult> Update(Guid id, U model, bool saveAndClose = true);
    }
}