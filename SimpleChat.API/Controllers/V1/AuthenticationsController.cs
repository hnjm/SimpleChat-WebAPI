using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleChat.API.Config;
using SimpleChat.Core;
using SimpleChat.Core.Auth;
using SimpleChat.Core.Helper;
using SimpleChat.Core.Validation;
using SimpleChat.Core.ViewModel;
using SimpleChat.Data.Service;
using SimpleChat.ViewModel.User;
using SimpleChat.Domain;
using SimpleChat.ViewModel.Auth;

namespace SimpleChat.API.Controllers.V1
{
    /// <summary>
    /// Purpose of the usage, when an user is unauthorized to
    /// create a new user record on the DB, and make available
    /// to use TokenController for futher needs.
    ///
    /// You should use TokenController after the registration of the new user.
    /// </summary>
    [ApiVersion("1.0")]
    [EnableCors(ConstantValues.DefaultAuthCorsPolicy)]
    [Route("api/[controller]/[action]")]
    [AllowAnonymous]
    public class AuthenticationsController : DefaultApiController
    {
        #region Properties and Fields

        private ITokenService _tokenService;
        private readonly IMapper _mapper;

        readonly UserManager<User> _userManager;
        readonly SignInManager<User> _signInManager;
        private readonly APIResult _apiResult;

        #endregion

        #region Ctor

#pragma warning disable 1591
        public AuthenticationsController(IUserService service,
            ITokenService tokenService,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IMapper mapper,
            APIResult apiResult)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _apiResult = apiResult;
        }
#pragma warning restore 1591

        #endregion

        /// <summary>
        /// Checks is the username or the email already exist into the database
        /// </summary>
        /// <param name="userName">Usename to be check on database for prevent a dupplication</param>
        /// <param name="eMail">EMail to be check on database for prevent a dupplication</param>
        /// <returns>Retruns result of DB check as boolen results with the username and email which are searched into the DB</returns>
        /// <response code="400">If userName parameter is null or empty, this is returns</response>
        /// <response code="200">DB query result</response>
        /// <response code="500">Empty payload with HTTP Status Code</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IsUserExistVM))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<JsonResult> IsUserExist([FromQuery] string userName, [FromQuery] string eMail)
        {
            if (userName.IsNullOrEmptyString() || eMail.IsNullOrEmptyString())
                return new JsonAPIResult("", StatusCodes.Status400BadRequest);

            var eMailResult = await _userManager.FindByEmailAsync(eMail);
            bool isEmailExist = eMailResult != null ||  (eMailResult != null && !eMailResult.Id.IsEmptyGuid());

            var userNameResult = await _userManager.FindByNameAsync(userName);
            bool isUserNameExist = userNameResult != null || (userNameResult != null && !userNameResult.Id.IsEmptyGuid());

            var model = new IsUserExistVM()
            {
                UserName = userName,
                EMail = eMail,
                IsEMailExist = isEmailExist,
                IsUserNameExist = isUserNameExist
            };

            if (isUserNameExist || isEmailExist)
                return new JsonAPIResult(model, StatusCodes.Status200OK);
            else
                return new JsonAPIResult(model, StatusCodes.Status200OK);
        }

        /// <summary>
        /// Creates a new user in the database
        /// </summary>
        /// <param name="model">Contains data for the new user</param>
        /// <returns>Data of the created user and the Token value</returns>
        /// <response code="400">If posted data by the client is not valid, it returns APIResultVM which is contains error-codes</response>
        /// <response code="201">Registered user data and the token returns</response>
        /// <response code="422">It is similar request like 400, it contatins Identity Validation Errors</response>
        /// <response code="500">Empty payload with HTTP Status Code</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(APIResultVM))]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(APIResultWithRecVM<UserAuthenticationVM>))]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(APIResultVM))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<JsonResult> Register([FromBody] UserRegisterVM model)
        {
            if (!ModelState.IsValid)
                return new JsonAPIResult(_apiResult.CreateVMWithModelState(modelStateDictionary: ModelState),
                    StatusCodes.Status400BadRequest);

            User entity = _mapper.Map<UserRegisterVM, User>(model);

            entity.Id = Guid.NewGuid();
            entity.CreateDateTime = DateTime.UtcNow;

            var claims = new Claim[] {
                new Claim(JwtRegisteredClaimNames.Sub, entity.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, entity.UserName),
                new Claim("UserId", entity.Id.ToString())
            };

            TokenCacheVM authData = new TokenCacheVM();
            authData.AccessToken = _tokenService.GenerateAccessToken(claims);
            authData.RefreshToken = _tokenService.GenerateRefreshToken();
            authData.AccessTokenExpiryTime = new JwtSecurityTokenHandler().ReadToken(authData.AccessToken)?.ValidTo ?? DateTime.MinValue;
            authData.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1);
            authData.Id = entity.Id;

            var identityResult = await _userManager.CreateAsync(entity, model.Password);
            if (!identityResult.Succeeded)
            return new JsonAPIResult(_apiResult.CreateVMWithIdentityErrors(errors: identityResult.Errors),
                    StatusCodes.Status422UnprocessableEntity);

            TimeSpan expiryTimeSpan = TimeSpan.FromSeconds(_tokenService.GetTokenExpiryDuration());
            var cacheResult = _tokenService.Redis.Insert(authData, expiryTimeSpan);
            if (!cacheResult.IsSuccessful)
                return new JsonAPIResult(_apiResult.CreateVMWithStatusCode(null, false, APIStatusCode.ERR01011),
                    StatusCodes.Status409Conflict);

            await _signInManager.SignInAsync(entity, isPersistent: false);

            UserAuthenticationVM returnVM = new UserAuthenticationVM();
            returnVM = _mapper.Map<User, UserAuthenticationVM>(entity);
            returnVM.TokenData = authData;

            return new JsonAPIResult(_apiResult.CreateVMWithRec<UserAuthenticationVM>(returnVM, entity.Id, true),
                StatusCodes.Status201Created);
        }
    }
}
