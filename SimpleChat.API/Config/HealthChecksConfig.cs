using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleChat.API.HealthChecks;
using SimpleChat.Data;

namespace SimpleChat.API.Config
{
    internal static class HealthChecksConfig
    {
        internal static void Add(ref IServiceCollection services, IConfiguration Configuration)
        {
            services.AddHealthChecks()
                .AddDbContextCheck<SimpleChatDbContext>()
                .AddCheck<RedisHealthCheck>("Redis");
        }
    }
}
