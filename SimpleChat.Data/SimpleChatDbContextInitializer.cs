using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sentry;
using SimpleChat.Core.Validation;
using SimpleChat.Data.Mapping;
using SimpleChat.Data.SubStructure;
using SimpleChat.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SimpleChat.Data
{
    public class SimpleChatDbContextInitializer
    {
        private readonly UserManager<User> _userManager;
        private readonly UnitOfWork _uow;

        public SimpleChatDbContextInitializer(UserManager<User> userManager,
            UnitOfWork uow)
        {
            _userManager = userManager;
            _uow = uow;
        }


        public async Task Seed()
        {
            try
            {
                #region Users

                var testUser = await _userManager.FindByNameAsync("testuser");
                if (testUser == null || testUser.Id.IsEmptyGuid())
                {

                    testUser = new User()
                    {
                        CreateDateTime = DateTime.UtcNow,
                        Id = Guid.NewGuid(),
                        DisplayName = "Test User",
                        Email = "testuser@simplechat.com",
                        NormalizedEmail = "TESTUSER@SIMPLECHAT.COM",
                        UserName = "testuser",
                        NormalizedUserName = "TESTUSER"
                    };

                    var testUserResult = await _userManager.CreateAsync(testUser, "Testuser.123456");
                    if (!testUserResult.Succeeded)
                        throw new Exception(JsonSerializer.Serialize(testUserResult.Errors));
                }

                #endregion

                #region ChatRooms

                var mainChatRoom = _uow.Repository<ChatRoom>().Query().Where(a => a.Name.Equals("Main")).FirstOrDefault();
                if (mainChatRoom == null)
                {
                    _uow.Repository<ChatRoom>().Add(new ChatRoom()
                    {
                        CreateBy = Guid.Empty,
                        CreateDT = DateTime.UtcNow,
                        Description = "",
                        Id = Guid.NewGuid(),
                        IsMain = true,
                        Name = "Main"
                    });
                }

                #endregion

                await _uow.SaveChangesAsync();
            }
            catch (System.Exception e)
            {
                SentrySdk.CaptureException(e);
            }
        }
    }
}
