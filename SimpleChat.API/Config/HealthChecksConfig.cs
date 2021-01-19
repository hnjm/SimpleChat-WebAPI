using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SimpleChat.API.HealthChecks;
using SimpleChat.Data;

namespace SimpleChat.API.Config
{
    internal static class HealthChecksConfig
    {
        internal static void Add(ref IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddCheck("Self", () => HealthCheckResult.Healthy())
                .AddDbContextCheck<SimpleChatDbContext>()
                .AddCheck<RedisHealthCheck>("Redis")
                .AddCheck<ChatHubHeathCheck>("SignalR (ChatHub)");
            services.AddHealthChecksUI(setupSettings: setup =>
                {
                    setup.SetEvaluationTimeInSeconds(5); //Configures the UI to poll for healthchecks updates every 5 seconds
                    setup.MaximumHistoryEntriesPerEndpoint(50);
                })
                .AddSqlServerStorage(configuration.GetConnectionString("HealthCheckConnection"),
                options =>
                {
                    options.EnableDetailedErrors(true);
                });
        }
    }
}
