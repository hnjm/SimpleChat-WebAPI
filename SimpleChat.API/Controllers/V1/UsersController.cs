using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SimpleChat.Core;
using SimpleChat.Core.Helper;
using SimpleChat.Core.Validation;
using SimpleChat.Core.ViewModel;
using SimpleChat.Data.Service;
using SimpleChat.ViewModel.Message;
using SimpleChat.ViewModel.User;
using SimpleChat.Domain;

namespace SimpleChat.API.Controllers.V1
{
    /// <summary>
    /// User CRUD operations, but without Register operation, Register manages by AuthenticationController
    /// </summary>
    [ApiVersion("1.0")]
    public class UsersController : DefaultApiController
    {
        #region Properties and Fields

        private IConfiguration _config;
        private IUserService _service;
        private readonly IMapper _mapper;

        readonly UserManager<User> _userManager;
        readonly SignInManager<User> _signInManager;
        private readonly APIResult _apiResult;

        #endregion

        #region Ctor

#pragma warning disable 1591
        public UsersController(IUserService service,
            IConfiguration config,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IMapper mapper,
            APIResult apiResult)
        {
            _config = config;
            _service = service;
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _apiResult = apiResult;
        }
#pragma warning restore 1591

        #endregion

        /// <summary>
        /// To get all records
        /// </summary>
        /// <returns>List of users</returns>
        /// <response code="404">If DB don't have any record</response>
        /// <response code="200">If any records exist on the DB</response>
        /// <response code="500">Empty payload with HTTP Status Code</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(APIResultVM))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(APIResultVM))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResultWithRecVM<IEnumerable<UserListVM>>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public JsonResult Get()
        {
            var result = _service.GetUserList();
            if (result == null)
                return new JsonAPIResult(_apiResult.CreateVMWithStatusCode(null, false, APIStatusCode.ERR01002),
                    StatusCodes.Status404NotFound);

            return new JsonAPIResult(result, StatusCodes.Status200OK);
        }

        /// <summary>
        /// To get only one specific record
        /// </summary>
        /// <param name="id">The function needs ID value of which records want by the client</param>
        /// <returns>Record data as UserVM, or error response</returns>
        /// <response code="400">When ID parameter is empty-guid or null</response>
        /// <response code="404">If there is no record with ID which is sent by client</response>
        /// <response code="200">Data of requested record</response>
        /// <response code="500">Empty payload with HTTP Status Code</response>
        [HttpGet()]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(APIResultVM))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(APIResultVM))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResultWithRecVM<UserVM>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public JsonResult GetById([FromRoute] Guid id)
        {
            if (id.IsEmptyGuid())
                return new JsonAPIResult(_apiResult.CreateVMWithStatusCode(null, false, APIStatusCode.ERR01003),
                    StatusCodes.Status400BadRequest);

            var user = _service.GetById(id);
            if (user == null)
                return new JsonAPIResult(_apiResult.CreateVMWithStatusCode(null, false, APIStatusCode.ERR01002),
                    StatusCodes.Status404NotFound);

            var result = _mapper.Map<UserVM>(user);

            return new JsonAPIResult(_apiResult.CreateVMWithRec<UserVM>(result, result.Id, true),
                StatusCodes.Status200OK);
        }

        /// <summary>
        /// To update authorized user
        /// </summary>
        /// <param name="id">id of the </param>
        /// <param name="model">Values to be updated for the user</param>
        /// <returns>A list of filtered messages</returns>
        /// <response code="400">When KEY parameter is empty or null</response>
        /// <response code="404">If there is no record which is contains KEY parameter value</response>
        /// <response code="403">If user tries to update to another user</response>
        /// <response code="200">List of messages</response>
        /// <response code="500">Empty payload with HTTP Status Code</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(APIResultVM))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(APIResultVM))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(APIResultVM))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResultWithRecVM<IEnumerable<MessageVM>>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<JsonResult> Update([FromRoute] Guid id, [FromBody] UserUpdateVM model)
        {
            if (id.IsEmptyGuid())
                return new JsonAPIResult(_apiResult.CreateVMWithStatusCode(null, false, APIStatusCode.ERR01003),
                    StatusCodes.Status400BadRequest);
            else if (!ModelState.IsValid)
                return new JsonAPIResult(_apiResult.CreateVMWithModelState(modelStateDictionary: ModelState),
                    StatusCodes.Status400BadRequest);

            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            if(currentUser == null)
                return new JsonAPIResult(_apiResult.CreateVMWithStatusCode(null, false, APIStatusCode.ERR01004),
                    StatusCodes.Status404NotFound);

            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return new JsonAPIResult(_apiResult.CreateVMWithStatusCode(null, false, APIStatusCode.ERR01004),
                    StatusCodes.Status404NotFound);

            if(currentUser.Id != user.Id)
                return new JsonAPIResult(_apiResult.CreateVMWithStatusCode(null, false, APIStatusCode.ERR01007),
                    StatusCodes.Status403Forbidden);

            user.About = model.About;
            user.DisplayName = model.DisplayName;

            IdentityResult result;

            result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return new JsonAPIResult(_apiResult.CreateVMWithStatusCode(null, false, APIStatusCode.ERR01010),
                    StatusCodes.Status422UnprocessableEntity);

            return new JsonAPIResult(_apiResult.CreateVM(user.Id, true), StatusCodes.Status200OK);
        }

        //TODO: ADD DELETE FUNCTION

    }
}
