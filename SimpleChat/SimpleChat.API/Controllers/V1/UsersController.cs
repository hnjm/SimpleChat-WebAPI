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
using SimpleChat.Core;
using SimpleChat.Core.Helper;
using SimpleChat.Core.Validation;
using SimpleChat.Data.Service;
using SimpleChat.Data.ViewModel.User;
using SimpleChat.Domain;

namespace SimpleChat.API.Controllers.V1
{
    [ApiVersion("1.0")]
    public class UsersController : DefaultApiController
    {
        private IConfiguration _config;
        private IUserService _service;
        private readonly IMapper _mapper;

        readonly UserManager<User> _userManager;
        readonly SignInManager<User> _signInManager;

        public UsersController(IUserService service,
            IConfiguration config,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IMapper mapper, ILogger<UsersController> logger)
             : base(logger)
        {
            _config = config;
            _service = service;
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }

  

        [HttpGet]
        public JsonResult Get()
        {
            try
            {
                var result = _service.GetUserList();

                if (result == null)
                    return new JsonResult(APIResult.CreateVMWithStatusCode(false, null, APIStatusCode.ERR01002));

                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return new JsonResult(APIResult.CreateVM());
            }
        }

        [HttpGet("{id}")]
        public JsonResult GetById(Guid id)
        {
            try
            {
                if (id.IsEmptyGuid())
                    return new JsonResult(APIResult.CreateVMWithStatusCode(false, null, APIStatusCode.ERR01002));

                var result = _service.GetById(id);

                if (result == null)
                    return new JsonResult(APIResult.CreateVMWithStatusCode(false, null, APIStatusCode.ERR01002));

                return new JsonResult(APIResult.CreateVMWithRec<User>(result, true, result.Id));
            }
            catch (Exception ex)
            {
                return new JsonResult(APIResult.CreateVM());
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

                    IdentityResult result;

                    //    if (model.OldPassword != model.PasswordHash)
                    //    {
                    //        result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.PasswordHash);
                    //        if (!result.Succeeded)
                    //            return new JsonResult(APIResult.CreateVM(false, null, APIStatusCode.ERR01002));
                    //    }
                    result = await _userManager.UpdateAsync(user);
                    if (!result.Succeeded)
                        return new JsonResult(APIResult.CreateVMWithStatusCode(false, null, APIStatusCode.ERR01002));
                }

                return new JsonResult(APIResult.CreateVM(true, user.Id));
            }
            catch (Exception ex)
            {
                return new JsonResult(APIResult.CreateVM());
            }
        }

    }
}
