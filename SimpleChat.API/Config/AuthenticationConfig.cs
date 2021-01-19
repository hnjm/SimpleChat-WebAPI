using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace SimpleChat.API.Config
{
    internal static class AuthenticationConfig
    {
        internal static void Add(ref IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<String>("Jwt:Key")));

                options.RequireHttpsMetadata = false;
                options.SaveToken = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    LifetimeValidator = (before, expires, token, param) =>
                    {
                        return expires > DateTime.UtcNow;
                    },
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ValidAudience = configuration["Jwt:Audience"],
                    ValidIssuer = configuration["Jwt:Issuer"],
                };

                //options.Events = new JwtBearerEvents
                //{
                //    OnMessageReceived = context =>
                //    {
                //        var accessToken = context.Request.Query["access_token"];

                //        var path = context.HttpContext.Request.Path;
                //        if (!string.IsNullOrEmpty(accessToken) &&
                //            (path.StartsWithSegments("/chatHub")))
                //        {
                //            context.Token = accessToken;
                //        }
                //        return Task.CompletedTask;
                //    }
                //};

                //services.AddSwaggerGen(c =>
                //{
                //    c.SwaggerDoc("v1", new Info { Title = "Values Api", Version = "v1" });
                //    c.AddSecurityDefinition("Bearer",
                //           new ApiKeyScheme
                //           {
                //               In = "header",
                //               Name = "Authorization",
                //               Type = "apiKey"
                //           });
                //    c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
                //     { "Bearer", Enumerable.Empty<string>() },
                //     });

                //});

            });
        }
    }
}
