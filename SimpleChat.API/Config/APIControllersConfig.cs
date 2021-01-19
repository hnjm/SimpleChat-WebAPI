using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleChat.API.Config
{
    internal static class APIControllersConfig
    {
        internal static void Add(ref IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers(options =>
            {
                options.Conventions.Add(new GroupingByNamespaceConvention());
                options.RespectBrowserAcceptHeader = false;
                options.ReturnHttpNotAcceptable = true;
                // For details: https://code-maze.com/content-negotiation-dotnet-core/
            }).ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });
        }
    }
}
