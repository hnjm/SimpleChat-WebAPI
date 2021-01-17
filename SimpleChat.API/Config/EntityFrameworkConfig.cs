using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleChat.Core.Validation;
using SimpleChat.Data;
using SimpleChat.Domain;

namespace SimpleChat.API.Config
{
    internal static class EntityFrameworkConfig
    {
        internal static void Add(ref IServiceCollection services, IConfiguration Configuration)
        {
            services.AddIdentity<User, Domain.Role>(x => x.User.RequireUniqueEmail = true)
                .AddErrorDescriber<IdentityErrorDescriberForAPIStatusCodes>()
                .AddEntityFrameworkStores<SimpleChatDbContext>()
                .AddDefaultTokenProviders();
        }
    }
}
