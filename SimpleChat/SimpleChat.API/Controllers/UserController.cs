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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NGA.Core;
using NGA.Core.Helper;
using NGA.Core.Model;
using NGA.Core.Validation;
using NGA.Data;
using NGA.Data.Service;
using NGA.Data.ViewModel;
using NGA.Domain;

namespace NGA.MonolithAPI.Controllers.V2
{
    public class UserController : DefaultApiController
    {
        private IConfiguration _config;
        private IUserService _service;
        private readonly IMapper _mapper;

        readonly UserManager<User> _userManager;
        readonly SignInManager<User> _signInManager;

        public UserController(IUserService service, IConfiguration config, UserManager<User> userManager,
            SignInManager<User> signInManager, IMapper mapper, ILogger<UserController> logger)
             : base(logger)
        {
            _config = config;
            _service = service;
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult UserNameIsExist(string userName)
        {
            try
            {
                var result = _service.Any(userName);

                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return new JsonResult(APIResult.CreateVMWithError(ex, APIResult.CreateVM(false, null, AppStatusCode.ERR01001)));
            }
        }

        [HttpGet]
        public JsonResult Get()
        {
            try
            {
                var result = _service.GetUserList();

                if (result == null)
                    return new JsonResult(APIResult.CreateVM(false, null, AppStatusCode.WRG01001));

                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return new JsonResult(APIResult.CreateVMWithError(ex, APIResult.CreateVM(false, null, AppStatusCode.ERR01001)));
            }
        }

        [HttpGet]
        public JsonResult GetById(Guid id)
        {
            try
            {
                if (Validation.IsNullOrEmpty(id))
                    return new JsonResult(APIResult.CreateVM(false, null, AppStatusCode.WRG01002));

                var result = _service.GetById(id);

                if (result == null)
                    return new JsonResult(APIResult.CreateVM(false, null, AppStatusCode.WRG01001));

                return new JsonResult(APIResult.CreateVMWithRec<User>(result, true, result.Id));
            }
            catch (Exception ex)
            {
                return new JsonResult(APIResult.CreateVMWithError(ex, APIResult.CreateVM(false, null, AppStatusCode.ERR01001)));
            }
        }

        [HttpPut]
        public async Task<JsonResult> Update(UserUpdateVM model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(model.Id.ToString());
                if (user != null)
                {
                    user.About = model.About;
                    user.DisplayName = model.DisplayName;
                    user.IsAdmin = model.IsAdmin;                    
                    user.UserName = model.UserName;

                    IdentityResult result;

                    if (model.OldPassword != model.PasswordHash)
                    {
                        result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.PasswordHash);
                        if (!result.Succeeded)
                            return new JsonResult(APIResult.CreateVM(false, null, AppStatusCode.WRG01001));
                    }
                    result = await _userManager.UpdateAsync(user);
                    if (!result.Succeeded)
                        return new JsonResult(APIResult.CreateVM(false, null, AppStatusCode.WRG01001));                    
                }

                return new JsonResult(APIResult.CreateVM(true, user.Id));
            }
            catch (Exception ex)
            {
                return new JsonResult(APIResult.CreateVMWithError(ex, APIResult.CreateVM(false, null, AppStatusCode.ERR01001)));
            }
        }

        [HttpGet]
        [AllowAnonymous]      
        public async Task<JsonResult> CreateToken(UserLoginVM model)
        {
            var loginResult = await _signInManager.PasswordSignInAsync(model.UserName, model.PasswordHash, isPersistent: false, lockoutOnFailure: false);

            if (!loginResult.Succeeded)
                return new JsonResult(APIResult.CreateVM(false, null, AppStatusCode.WRG01004));

            var user = await _userManager.FindByNameAsync(model.UserName);
            user.LastLoginDateTime = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            UserAuthenticateVM returnVM = new UserAuthenticateVM();
            returnVM = _mapper.Map<User, UserAuthenticateVM>(user);
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

        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> Register(UserAddVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    User entity = _mapper.Map<UserAddVM, User>(model);
                    entity.Id = Guid.NewGuid();
                    entity.CreateDateTime = DateTime.UtcNow;
                    entity.LastLoginDateTime = DateTime.UtcNow;
                    var identityResult = await _userManager.CreateAsync(entity, model.PasswordHash);
                    if (identityResult.Succeeded)
                    {
                        await _signInManager.SignInAsync(entity, isPersistent: false);


                        UserAuthenticateVM returnVM = new UserAuthenticateVM();
                        returnVM = _mapper.Map<User, UserAuthenticateVM>(entity);
                        returnVM.Token = GetToken(entity);

                        return new JsonResult(returnVM);
                    }
                    else
                    {
                        return new JsonResult(APIResult.CreateVMWithRec<object>(identityResult.Errors));
                    }
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(APIResult.CreateVMWithError(ex, APIResult.CreateVM(false, null, AppStatusCode.ERR01001)));
            }
            return new JsonResult(APIResult.CreateVM(false, null, AppStatusCode.ERR01001));
        }

        [HttpGet]
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
    }
}
