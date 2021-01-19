using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sentry;
using Sentry.AspNetCore;
using Sentry.Extensibility;
using Sentry.Protocol;

namespace SimpleChat.API.Config
{
    internal static class SentryConfig
    {
        internal static SentryAspNetCoreOptions GetOptions()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile(
                    $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
                    optional: true)
                .Build();

            SentryAspNetCoreOptions options = new SentryAspNetCoreOptions();

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

            return options;
        }
    }
}
