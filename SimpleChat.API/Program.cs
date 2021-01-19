using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using SimpleChat.API.Config;

namespace SimpleChat.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.UseSentry(options => options = SentryConfig.GetOptions());
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
}
