using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleChat.Core;
using SimpleChat.Core.EntityFramework;
using SimpleChat.Core.Helper;
using SimpleChat.Core.Validation;
using SimpleChat.Core.ViewModel;
using SimpleChat.Data.SubStructure;

namespace SimpleChat.API.Controllers.V1
{
    public interface IDefaultApiCRUDController<A, U, L, D, S>
            where A : AddVM, IAddVM, new()
            where U : UpdateVM, IUpdateVM, new()
            where L : BaseVM, IBaseVM, new()
            where D : BaseEntity, IBaseEntity, new()
            where S : BaseService<A, U, L, D>, IBaseService<A, U, L, D>
    {
        JsonResult Get();
        Task<JsonResult> GetById(Guid id);
        Task<JsonResult> Add(A model);
        Task<JsonResult> Update(Guid id, U model);
        Task<JsonResult> Delete(Guid id);
    }

    public abstract class DefaultApiCRUDController<A, U, L, D, S> : DefaultApiController, IDefaultApiCRUDController<A, U, L, D, S>
            where A : AddVM, IAddVM, new()
            where U : UpdateVM, IUpdateVM, new()
            where L : BaseVM, IBaseVM, new()
            where D : BaseEntity, IBaseEntity, new()
            where S : BaseService<A, U, L, D>, IBaseService<A, U, L, D>
    {
        protected S _service;

        public DefaultApiCRUDController(S service, ILogger<DefaultApiCRUDController<A, U, L, D, S>> logger)
            : base(logger)
        {
            _service = service;
        }

        [HttpGet]
        public virtual JsonResult Get()
        {
            try
            {
                var result = _service.GetAllAsync();             

                if (result == null)
                    return new JsonResult(APIResult.CreateVM(false, null, APIStatusCode.WRG01001));

                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new JsonResult(APIResult.CreateVM());
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
                if (id.IsEmptyGuid())
                    return new JsonResult(APIResult.CreateVM(false, null, APIStatusCode.WRG01002));

                var result = await _service.GetByIdAsync(id);

                if (result == null)
                    return new JsonResult(APIResult.CreateVM(false, null, APIStatusCode.WRG01001));

                return new JsonResult(APIResult.CreateVMWithRec<L>(result, true, result.Id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                //TODO: LOGGING
                //return new JsonResult(APIResult.CreateVMWithError(ex, APIResult.CreateVM(false, null, APIStatusCode.ERR01001)));
                return new JsonResult(APIResult.CreateVM());
            }
        }

        [HttpPost]
        public virtual async Task<JsonResult> Add(A model)
        {
            APIResultVM result = new APIResultVM();

            try
            {
                if (model.IsNull())
                    APIResult.CreateVM(false, null, APIStatusCode.WRG01001);

                result = await _service.AddAsync(model);

                return new JsonResult(APIResult.CreateVM(result.ResultIsTrue(), result.RecId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new JsonResult(APIResult.CreateVM());
            }
        }

        [HttpPut]
        public virtual async Task<JsonResult> Update(Guid id, U model)
        {
            APIResultVM result = new APIResultVM();

            try
            {
                if (model.IsNull())
                    APIResult.CreateVM(false, id, APIStatusCode.WRG01001);

                result = await _service.UpdateAsync(id, model);

                return new JsonResult(APIResult.CreateVM(result.ResultIsTrue(), result.RecId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new JsonResult(APIResult.CreateVM());
            }
        }

        [HttpDelete]
        public virtual async Task<JsonResult> Delete(Guid id)
        {
            APIResultVM result = new APIResultVM();

            try
            {
                if (id.IsEmptyGuid())
                    APIResult.CreateVM(false, null, APIStatusCode.WRG01001);

                result = await _service.DeleteAsync(id);

                return new JsonResult(APIResult.CreateVM(result.ResultIsTrue(), result.RecId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new JsonResult(APIResult.CreateVM());
            }
        }
    }

}