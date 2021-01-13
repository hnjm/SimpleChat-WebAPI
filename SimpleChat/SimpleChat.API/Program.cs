using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SimpleChat.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    // webBuilder.UseKestrel(options =>
                    // {
                    //     options.Listen(IPAddress.Loopback, 5060, listenOptions =>
                    //     {
                    //         listenOptions.UseHttps("./SimpleChat.API.crt", "SIMPLECHAT");
                    //     });
                    // });
                });
    }
}
