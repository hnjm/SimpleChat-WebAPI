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
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SimpleChat.API.Config;
using SimpleChat.Core;
using SimpleChat.Core.Helper;
using SimpleChat.Core.Validation;
using SimpleChat.Core.ViewModel;
using SimpleChat.Data.Service;
using SimpleChat.Data.ViewModel.User;
using SimpleChat.Domain;

namespace SimpleChat.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [EnableCors(ConstantValues.DefaultAuthCorsPolicy)]
    [Route("api/[controller]/[action]")]
    [AllowAnonymous]
    public class AuthenticationsController : DefaultApiController
    {
        private IConfiguration _config;
        private IUserService _service;
        private readonly IMapper _mapper;

        readonly UserManager<User> _userManager;
        readonly SignInManager<User> _signInManager;

        public AuthenticationsController(IUserService service,
            IConfiguration config,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IMapper mapper,
            ILogger<AuthenticationsController> logger)
             : base(logger)
        {
            _config = config;
            _service = service;
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        /// <summary>
        /// Checks the username already exist in to the database
        /// </summary>
        /// <param name="userName">Usename which is want to be check on database for prevent a dupplication</param>
        /// <returns>Retruns result of DB check as boolen and the username which is searched int to the DB</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<UserNameCheckResultVM>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(List<UserNameCheckResultVM>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserNameCheckResultVM>> UserNameIsExist(string userName)
        {
            var isExist = await _service.AnyAsync(userName);
            var model = new UserNameCheckResultVM()
            { UserName = userName, IsExist = isExist };

            if (isExist)
                return Ok(model);
            else
                return NotFound(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IEnumerable<string>))]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserAuthenticationVM))]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(IEnumerable<IdentityError>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<JsonResult> Register(UserRegisterVM model)
        {
            if (!ModelState.IsValid)
                return new JsonResult(APIResult.CreateVMWithModelState(modelStateDictionary: ModelState));

            User entity = _mapper.Map<UserRegisterVM, User>(model);

            entity.Id = Guid.NewGuid();
            entity.CreateDateTime = DateTime.UtcNow;

            var identityResult = await _userManager.CreateAsync(entity, model.Password);

            if (identityResult.Succeeded)
            {
                await _signInManager.SignInAsync(entity, isPersistent: false);

                UserAuthenticationVM returnVM = new UserAuthenticationVM();
                returnVM = _mapper.Map<User, UserAuthenticationVM>(entity);
                returnVM.Token = GetToken(entity);

                return Created("", APIResult.CreateVMWithRec<UserAuthenticationVM>(returnVM, true, entity.Id));
            }
            else
            {
                return UnprocessableEntity(APIResult.CreateVMWithIdentityErrors(errors: identityResult.Errors));
            }
        }

        [HttpGet]

        public async Task<JsonResult> CreateToken(UserLoginVM model)
        {
            var loginResult = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, isPersistent: false, lockoutOnFailure: false);

            if (!loginResult.Succeeded)
                return new JsonResult(APIResult.CreateVMWithStatusCode(false, null, APIStatusCode.ERR01004));

            var user = await _userManager.FindByNameAsync(model.UserName);
            user.LastLoginDateTime = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            var returnVM = _mapper.Map<User, UserAuthenticationVM>(user);
            returnVM.Token = GetToken(user);

            return new JsonResult(returnVM);
        }

        [HttpGet]
        public async Task<JsonResult> RefreshToken()
        {
            var user = await _userManager.FindByNameAsync(
                User.Identity.Name ??
                User.Claims.Where(c => c.Properties.ContainsKey("unique_name")).Select(c => c.Value).FirstOrDefault()
                );

            return new JsonResult(APIResult.CreateVMWithRec<string>(GetToken(user), true));
        }

        #region Helper Methods

        private String GetToken(User user)
        {
            var utcNow = DateTime.UtcNow;

            var claims = new Claim[]
            {
                        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                        new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, utcNow.ToString())
            };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetValue<String>("Jwt:Key")));

            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var jwt = new JwtSecurityToken(
                signingCredentials: signingCredentials,
                claims: claims,
                notBefore: utcNow,
                expires: utcNow.AddSeconds(_config.GetValue<int>("Jwt:ExpiryDuration")),
                audience: _config.GetValue<String>("Jwt:Audience"),
                issuer: _config.GetValue<String>("Jwt:Issuer")
                );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        #endregion
    }
}
