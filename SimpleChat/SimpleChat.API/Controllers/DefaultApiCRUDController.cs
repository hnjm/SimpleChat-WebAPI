using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleChat.Core.ViewModel;

namespace NGA.MonolithAPI.Controllers.V2
{
    public abstract class DefaultApiCRUDController<A, U, G, S> : DefaultApiController
             where A : AddVM, IAddVM, new()
             where U : UpdateVM, IUpdateVM, new()
             where G : BaseVM, IBaseVM, new()
             where S : ICRUDService<A, U, G>
    {
        protected S _service;

        public DefaultApiCRUDController(S service, ILogger<DefaultApiCRUDController<A, U, G, S>> logger)
            : base(logger)
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

                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new JsonResult(APIResult.CreateVMWithError(ex, APIResult.CreateVM(false, null, AppStatusCode.ERR01001)));
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(JsonResult), StatusCodes.Status500InternalServerError)]
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
                _logger.LogError(ex.ToString());
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
                _logger.LogError(ex.ToString());
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
                _logger.LogError(ex.ToString());
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
                _logger.LogError(ex.ToString());
                return new JsonResult(APIResult.CreateVMWithError(ex, APIResult.CreateVM(false, result.RecId)));
            }
        }
    }

    public interface IDefaultApiCRUDController<A, U, G, S>
            where A : AddVM, IAddVM, new()
            where U : UpdateVM, IUpdateVM, new()
            where G : BaseVM, IBaseVM, new()
            where S : ICRUDService<A, U, G>
    {
        JsonResult Get();
        Task<JsonResult> GetById(Guid id);
        Task<JsonResult> Add(A model);
        Task<JsonResult> Update(Guid id, U model);
        Task<JsonResult> Delete(Guid id);
    }
}