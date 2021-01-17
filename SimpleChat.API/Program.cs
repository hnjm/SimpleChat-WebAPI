using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sentry.Extensibility;
using Sentry.Protocol;
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

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile(
                    $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
                    optional: true)
                .Build();

            return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.UseSentry(options =>
                {
                    options.Dsn = configuration["Sentry:Dsn"];

                    bool.TryParse(configuration["Sentry:IncludeRequestPayload"], out bool includeRequestPayload);
                    options.MaxRequestBodySize = includeRequestPayload ? RequestSize.Always : RequestSize.None;

                    bool.TryParse(configuration["Sentry:SendDefaultPii"], out bool sendDefaultPii);
                    options.SendDefaultPii = sendDefaultPii;

                    Enum.TryParse<LogLevel>(configuration["Sentry:MinimumBreadcrumbLevel"], true, out LogLevel logLevel);
                    options.MinimumBreadcrumbLevel = logLevel;

                    Enum.TryParse<LogLevel>(configuration["Sentry:MinimumEventLevel"], true, out LogLevel minimumEventLevel);
                    options.MinimumEventLevel = minimumEventLevel;

                    bool.TryParse(configuration["Sentry:AttachStacktrace"], out bool attachStacktrace);
                    options.AttachStacktrace = attachStacktrace;

                    bool.TryParse(configuration["Sentry:Debug"], out bool debug);
                    options.Debug = debug;

                    Enum.TryParse<SentryLevel>(configuration["Sentry:DiagnosticsLevel"], true, out SentryLevel diagnosticsLevel);
                    options.DiagnosticsLevel = diagnosticsLevel;

                    options.BeforeSend = @event =>
                    {
                        // Never report server names
                        @event.ServerName = null;
                        return @event;
                    };
                });
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
