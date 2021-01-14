using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Sentry;
using Sentry.Protocol;
using SimpleChat.API.Config;
using SimpleChat.Core;
using SimpleChat.Core.Auth;
using SimpleChat.Core.Auth.ViewModel;
using SimpleChat.Core.Helper;
using SimpleChat.Core.Validation;
using SimpleChat.Core.ViewModel;
using SimpleChat.Data.Service;
using SimpleChat.Data.ViewModel.User;
using SimpleChat.Domain;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace SimpleChat.API.Controllers.V1
{
    /// <summary>
    /// Operates token operations, like create, refresh and revoke
    /// </summary>
    [ApiVersion("1.0")]
    [EnableCors(ConstantValues.DefaultAuthCorsPolicy)]
    [Route("api/[controller]/[action]")]
    [AllowAnonymous]
    public class TokensController : DefaultApiController
    {
        #region Properties and Fields

        private ITokenService _tokenService;
        private readonly IMapper _mapper;

        readonly UserManager<Domain.User> _userManager;
        readonly SignInManager<Domain.User> _signInManager;

        #endregion

        #region Ctor

#pragma warning disable 1591
        public TokensController(IUserService service,
            ITokenService tokenService,
            UserManager<Domain.User> userManager,
            SignInManager<Domain.User> signInManager,
            IMapper mapper,
            ILogger<TokensController> logger)
             : base(logger)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }
#pragma warning restore 1591

        #endregion

        /// <summary>
        /// Creates a token for a valid user
        /// </summary>
        /// <param name="model">Contains data for the verifying the user credentials</param>
        /// <returns>Data of the authenticated user and the Token value</returns>
        /// <response code="400">If posted data is not valid, it returns APIResultVM which is contains error-codes</response>
        /// <response code="404">If the user credentials not matched with any registered users</response>
        /// <response code="200">If the user successfully authenticated</response>
        /// <response code="500">Empty payload with HTTP Status Code</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(APIResultVM))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(APIResultVM))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserAuthenticationVM))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<JsonResult> Create([FromBody] UserLoginVM model)
        {
            if (!ModelState.IsValid)
                return new JsonAPIResult(APIResult.CreateVMWithModelState(modelStateDictionary: ModelState),
                    StatusCodes.Status400BadRequest);

            var loginResult = await _signInManager.PasswordSignInAsync(model.UserName, model.Password,
                 isPersistent: false, lockoutOnFailure: false);
            if (!loginResult.Succeeded)
                return new JsonAPIResult(APIResult.CreateVMWithStatusCode(false, null, APIStatusCode.ERR01004),
                    StatusCodes.Status404NotFound);

            var user = await _userManager.FindByNameAsync(model.UserName);

            if (!user.AccessToken.IsNullOrEmptyString() && user.AccessTokenExpiryTime > DateTime.UtcNow)
            {
                user.LastLoginDateTime = DateTime.UtcNow;
            }
            else
            {
                var claims = new Claim[] {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                    new Claim("UserId", user.Id.ToString())
                };

                user.LastLoginDateTime = DateTime.UtcNow;
                user.AccessToken = _tokenService.GenerateAccessToken(claims);
                user.AccessTokenExpiryTime = new JwtSecurityTokenHandler().ReadToken(user.AccessToken)?.ValidTo ?? DateTime.MinValue;
                user.RefreshToken = _tokenService.GenerateRefreshToken();
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1);
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return new JsonAPIResult(APIResult.CreateVMWithStatusCode(false, null, APIStatusCode.ERR01001),
                    StatusCodes.Status404NotFound);

            var returnVM = _mapper.Map<Domain.User, UserAuthenticationVM>(user);

            SentrySdk.CaptureMessage($"User {user.UserName} is created an access token", SentryLevel.Info);
            return new JsonAPIResult(returnVM, StatusCodes.Status200OK);
        }

        /// <summary>
        /// Creates new access token and refresh token for a valid user
        /// </summary>
        /// <param name="model">Contains values of the access and the refresh tokens</param>
        /// <returns>New access token and new refresh token</returns>
        /// <response code="400">If posted data is not valid, it returns APIResultVM which is contains error-codes</response>
        /// <response code="404">If the tokens not attached to any registered users</response>
        /// <response code="409">If the token value cant updated on the user record</response>
        /// <response code="200">If the tokens successfully refreshed</response>
        /// <response code="500">Empty payload with HTTP Status Code</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(APIResultVM))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(APIResultVM))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(APIResultVM))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenRefreshVM))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<JsonResult> Refresh([FromBody] TokenRefreshVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return new JsonAPIResult(APIResult.CreateVMWithModelState(modelStateDictionary: ModelState),
                        StatusCodes.Status400BadRequest);


                ClaimsPrincipal principal = new ClaimsPrincipal();
                try
                {
                    principal = _tokenService.GetPrincipalFromExpiredToken(model.AccessToken);
                }
                catch (SecurityTokenException ex)
                {
                    SentrySdk.CaptureException(ex);
                    return new JsonAPIResult(APIResult.CreateVMWithStatusCode(statusCode: APIStatusCode.ERR02026),
                        StatusCodes.Status400BadRequest);
                }

                var user = await _userManager.FindByNameAsync(principal.Identity.Name);
                if (user == null)
                    return new JsonAPIResult(APIResult.CreateVMWithStatusCode(statusCode: APIStatusCode.ERR01004),
                        StatusCodes.Status404NotFound);
                else if (!user.AccessToken.IsNullOrEmptyString() && user.AccessTokenExpiryTime > DateTime.UtcNow)
                    return new JsonAPIResult(APIResult.CreateVMWithStatusCode(statusCode: APIStatusCode.ERR02025),
                            StatusCodes.Status400BadRequest);

                if (user.RefreshToken != model.RefreshToken)
                    return new JsonAPIResult(APIResult.CreateVMWithStatusCode(statusCode: APIStatusCode.ERR02024),
                        StatusCodes.Status400BadRequest);
                if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                    return new JsonAPIResult(APIResult.CreateVMWithStatusCode(statusCode: APIStatusCode.ERR02023),
                        StatusCodes.Status400BadRequest);

                principal.Claims.Append(new Claim("UserId", user.Id.ToString()));

                var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
                var newRefreshToken = _tokenService.GenerateRefreshToken();

                user.AccessToken = newAccessToken;
                user.RefreshToken = newRefreshToken;
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                    return new JsonAPIResult(APIResult.CreateVMWithStatusCode(false, null, APIStatusCode.ERR01001),
                        StatusCodes.Status409Conflict);

                SentrySdk.CaptureMessage($"User: {user.UserName} refreshed its access token", SentryLevel.Info);
                return new JsonAPIResult(APIResult.CreateVMWithRec<TokenRefreshVM>(new TokenRefreshVM()
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                }, true), StatusCodes.Status200OK);
            }
            catch (SecurityTokenException stEx)
            {
                SentrySdk.CaptureException(stEx);
                return new JsonAPIResult("", StatusCodes.Status500InternalServerError);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return new JsonAPIResult("", StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Removes the refresh token from the user record, that means token wont be useable after this proccess
        /// </summary>
        /// <returns>Http Status Codes to inform client about the proccess how handled</returns>
        /// <response code="400">If the user not authenticated</response>
        /// <response code="409">If the token value cant updated on the user record</response>
        /// <response code="204">When the refresh token is cleans successfully</response>
        /// <response code="500">Empty payload with HTTP Status Code</response>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(APIResultVM))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(APIResultVM))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenRefreshVM))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Revoke()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (user == null)
                return new JsonAPIResult(APIResult.CreateVMWithStatusCode(statusCode: APIStatusCode.ERR01004),
                        StatusCodes.Status400BadRequest);

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = DateTime.MinValue;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return new JsonAPIResult(APIResult.CreateVMWithStatusCode(false, null, APIStatusCode.ERR01001),
                    StatusCodes.Status409Conflict);

            SentrySdk.CaptureMessage($"Refresh token of the user({user.UserName}) is revoked", SentryLevel.Info);
            return NoContent();
        }

        /// <summary>
        /// Checks is the access token is valid
        /// </summary>
        /// <returns>Response code, that depends to the token validity</returns>
        /// <response code="401">If access token is not valid</response>
        /// <response code="200">If access token is valid</response>
        /// <response code="500">Empty payload with HTTP Status Code</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(APIResultVM))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserAuthenticationVM))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Validate()
        {
            var accessToken = HttpContext.Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");

            if (User == null || User.Identity == null || !User.Identity.IsAuthenticated || User.Claims == null
                || accessToken.IsNullOrEmptyString())
                return Unauthorized();

            bool isTokenValid = _tokenService.IsTokenValid(accessToken);

            if (!isTokenValid)
                return Unauthorized();

            return Ok();
        }
    }
}