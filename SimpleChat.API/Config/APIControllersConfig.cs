using System;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
namespace SimpleChat.API.Config
{
    internal static class APIControllersConfig
    {
        internal static void Add(ref IServiceCollection services, IConfiguration configuration)
        {
            services.AddMvc().AddNewtonsoftJson();
            services.AddControllers(options =>
            {
                options.Conventions.Add(new GroupingByNamespaceConvention());
                options.RespectBrowserAcceptHeader = false;
                options.ReturnHttpNotAcceptable = true;
                // For details: https://code-maze.com/content-negotiation-dotnet-core/
            }).ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
        }
    }
}
