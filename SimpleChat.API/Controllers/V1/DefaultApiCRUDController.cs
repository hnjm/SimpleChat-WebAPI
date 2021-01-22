using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleChat.Core;
using SimpleChat.Core.Auth;
using SimpleChat.Core.EntityFramework;
using SimpleChat.Core.Helper;
using SimpleChat.Core.Validation;
using SimpleChat.Core.ViewModel;
using SimpleChat.Data.SubStructure;

namespace SimpleChat.API.Controllers.V1
{
    /// <summary>
    /// Contains definitions of CRUD functions for the DefaultApiCRUDController
    /// </summary>
    /// <typeparam name="A">AddVM</typeparam>
    /// <typeparam name="U">UpdateVM</typeparam>
    /// <typeparam name="L">BaseVM for get operations</typeparam>
    /// <typeparam name="D">Domain/Poco class</typeparam>
    /// <typeparam name="S">Service</typeparam>
    public interface IDefaultApiCRUDController<A, U, L, D, S>
            where A : AddVM, IAddVM, new()
            where U : UpdateVM, IUpdateVM, new()
            where L : BaseVM, IBaseVM, new()
            where D : BaseEntity, IBaseEntity, new()
            where S : IBaseService<A, U, L, D>
    {
        /// <summary>
        /// Use to get all records
        /// </summary>
        /// <returns>List of typeparam L(BaseVM)</returns>
        Task<JsonResult> Get();

        /// <summary>
        /// To get only one specific record
        /// </summary>
        /// <param name="id">The function needs ID value of which records want by the client</param>
        /// <returns>Record data as typeparam L, or error response</returns>
        Task<JsonResult> GetById(Guid id);

        /// <summary>
        /// To save new record to the DB
        /// </summary>
        /// <param name="model">an AddVM instance</param>
        /// <returns>When successfly record adds to the DB, it returns Id value of the new record</returns>
        Task<JsonResult> Add(A model);

        /// <summary>
        /// To update for an existing record on the DB
        /// </summary>
        /// <param name="id">ID of an existing record</param>
        /// <param name="model">Data of existing record with changed values</param>
        /// <returns>When successfly record updates on the DB, it returns Id value of the record</returns>
        Task<JsonResult> Update(Guid id, U model);

        /// <summary>
        /// Deletes a record from the DB
        /// </summary>
        /// <param name="id">ID points which record wants to be deleted by client</param>
        /// <returns>APIResultVM with errors or ID value, that depends to the success of delete operation</returns>
        Task<JsonResult> Delete(Guid id);
    }

    /// <summary>
    /// Inherits the DefaultAPIController and adds the CRUD functions
    /// </summary>
    /// <typeparam name="A">AddVM</typeparam>
    /// <typeparam name="U">UpdateVM</typeparam>
    /// <typeparam name="L">BaseVM for get operations</typeparam>
    /// <typeparam name="D">Domain/Poco class</typeparam>
    /// <typeparam name="S">Service</typeparam>
    public abstract class DefaultApiCRUDController<A, U, L, D, S> : DefaultApiController, IDefaultApiCRUDController<A, U, L, D, S>
            where A : AddVM, IAddVM, new()
            where U : UpdateVM, IUpdateVM, new()
            where L : BaseVM, IBaseVM, new()
            where D : BaseEntity, IBaseEntity, new()
            where S : IBaseService<A, U, L, D>
    {
        #region Properties and Fields

        /// <summary>
        /// IBaseService instance for the related typeparam
        /// </summary>
        protected S _service;
        /// <summary>
        /// Creates APIResultVM instances for responses
        /// </summary>
        protected readonly APIResult _apiResult;

        #endregion

        #region Ctor

#pragma warning disable 1591
        public DefaultApiCRUDController(S service, APIResult apiResult)
        {
            _service = service;
            _apiResult = apiResult;
        }
#pragma warning restore 1591

        #endregion

        /// <summary>
        /// To get all records
        /// </summary>
        /// <returns>List of typeparam L(BaseVM)</returns>
        /// <response code="404">If DB don't have any record</response>
        /// <response code="200">If any records exist on the DB</response>
        /// <response code="500">Empty payload with HTTP Status Code</response>
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(APIResultVM))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<IBaseVM>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public virtual async Task<JsonResult> Get()
        {
            var result = await _service.GetAllAsync();
            if (result == null)
                return new JsonAPIResult(_apiResult.CreateVMWithStatusCode(null, false, APIStatusCode.ERR01003),
                    StatusCodes.Status404NotFound);

            return new JsonAPIResult(result, StatusCodes.Status200OK);
        }

        /// <summary>
        /// To get only one specific record
        /// </summary>
        /// <param name="id">The function needs ID value of which records want by the client</param>
        /// <returns>Record data as typeparam L, or error response</returns>
        /// <response code="400">When ID parameter is empty-guid or null</response>
        /// <response code="404">If there is no record with ID which is sent by client</response>
        /// <response code="200">Data of requested record</response>
        /// <response code="500">Empty payload with HTTP Status Code</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(APIResultVM))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(APIResultVM))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BaseVM>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public virtual async Task<JsonResult> GetById([FromRoute] Guid id)
        {
            if (id.IsEmptyGuid())
                return new JsonAPIResult(_apiResult.CreateVMWithStatusCode(null, false, APIStatusCode.ERR01003),
                    StatusCodes.Status400BadRequest);

            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return new JsonAPIResult(_apiResult.CreateVMWithStatusCode(null, false, APIStatusCode.ERR01003),
                    StatusCodes.Status404NotFound);

            return new JsonAPIResult(result, StatusCodes.Status200OK);
        }

        /// <summary>
        /// To save new record to the DB
        /// </summary>
        /// <param name="model">an AddVM instance</param>
        /// <returns>When successfly record adds to the DB, it returns Id value of the new record</returns>
        /// <response code="400">When model is not valid</response>
        /// <response code="422">If new record couldnt save to the DB</response>
        /// <response code="200">Id value of the new record</response>
        /// <response code="500">Empty payload with HTTP Status Code</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(APIResultVM))]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(APIResultVM))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResultVM))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public virtual async Task<JsonResult> Add([FromBody] A model)
        {
            if (!ModelState.IsValid)
                return new JsonAPIResult(_apiResult.CreateVMWithModelState(modelStateDictionary: ModelState),
                    StatusCodes.Status400BadRequest);

            var result = await _service.AddAsync(model, User.GetId());
            if (result.ResultIsNotTrue())
                return new JsonAPIResult(result, StatusCodes.Status422UnprocessableEntity);

            return new JsonAPIResult(_apiResult.CreateVM(result.RecId, result.ResultIsTrue()),
                StatusCodes.Status200OK);
        }

        /// <summary>
        /// To update for an existing record on the DB
        /// </summary>
        /// <param name="id">ID of an existing record</param>
        /// <param name="model">Data of existing record with changed values</param>
        /// <returns>When successfly record updates on the DB, it returns Id value of the record</returns>
        /// <response code="400">When model is not valid or ID is null/empty</response>
        /// <response code="422">If new record couldnt update on the DB</response>
        /// <response code="200">If record updated successfully</response>
        /// <response code="500">Empty payload with HTTP Status Code</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(APIResultVM))]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(APIResultVM))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResultVM))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public virtual async Task<JsonResult> Update([FromRoute] Guid id, [FromBody] U model)
        {
            if (!ModelState.IsValid)
                return new JsonAPIResult(_apiResult.CreateVMWithModelState(modelStateDictionary: ModelState),
                    StatusCodes.Status400BadRequest);
            else if (id.IsEmptyGuid())
                return new JsonAPIResult(_apiResult.CreateVMWithStatusCode(statusCode: APIStatusCode.ERR01003),
                    StatusCodes.Status400BadRequest);

            var result = await _service.UpdateAsync(id, model, User.GetId());
            if (result.ResultIsNotTrue())
                return new JsonAPIResult(result, StatusCodes.Status422UnprocessableEntity);

            return new JsonAPIResult(_apiResult.CreateVM(result.RecId, result.ResultIsTrue()),
                StatusCodes.Status200OK);
        }

        /// <summary>
        /// Deletes a record from the DB
        /// </summary>
        /// <param name="id">ID points which record wants to be deleted by client</param>
        /// <returns>APIResultVM with errors or ID value, that depends to the success of delete operation</returns>
        /// <response code="400">When ID parameter is null or empty</response>
        /// <response code="409">If the record couldnt delete from the DB</response>
        /// <response code="204">If the record deleted as successfully</response>
        /// <response code="500">Empty payload with HTTP Status Code</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(APIResultVM))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(APIResultVM))]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(APIResultVM))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public virtual async Task<JsonResult> Delete([FromRoute] Guid id)
        {
            if (id.IsEmptyGuid())
                return new JsonAPIResult(_apiResult.CreateVMWithStatusCode(statusCode: APIStatusCode.ERR01003),
                    StatusCodes.Status400BadRequest);

            var result = await _service.DeleteAsync(id, User.GetId());
            if (result.ResultIsNotTrue())
                return new JsonAPIResult(result, StatusCodes.Status409Conflict);

            return new JsonAPIResult(_apiResult.CreateVM(result.RecId, result.ResultIsTrue()),
                StatusCodes.Status204NoContent);
        }
    }

}