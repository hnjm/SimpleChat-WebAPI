using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Sentry;
using SimpleChat.Data;
using StackExchange.Redis;

namespace SimpleChat.API.HealthChecks
{
    internal class RedisHealthCheck : IHealthCheck
    {
        private readonly RedisDbContext _dbContext;

        public RedisHealthCheck(RedisDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var database = _dbContext.GetDatabase();
                database.StringGet("health");
                return await Task.FromResult(HealthCheckResult.Healthy());
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
                return await Task.FromResult(HealthCheckResult.Unhealthy());
            }
        }
    }
}
