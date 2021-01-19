using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleChat.API.Config
{
    internal static class CorsConfig
    {
        internal static void Add(ref IServiceCollection services, IConfiguration configuration)
        {
            var allowedOrigins = configuration.GetValue<string>("AllowedOrigins")?.Split(",") ?? new string[0];

            services.AddCors(options =>
            {
                options.AddPolicy(ConstantValues.DefaultCorsPolicy, builder =>
                    builder.AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed(origin => true) // allow any origin
                    .AllowCredentials()
                    .SetPreflightMaxAge(new TimeSpan(0, 10, 0)));
                options.AddPolicy(ConstantValues.DefaultAuthCorsPolicy, builder =>
                    builder.AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed(origin => true) // allow any origin
                    .AllowCredentials()
                    .SetPreflightMaxAge(new TimeSpan(0, 10, 0)));
            });
        }
    }
}
