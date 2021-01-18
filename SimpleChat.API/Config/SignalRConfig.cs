using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SignalR;


namespace SimpleChat.API.Config
{
    internal static class SignalRConfig
    {
        internal static void Add(ref IServiceCollection services, IConfiguration configuration)
        {
            services.AddSignalR();
        }
    }
}
