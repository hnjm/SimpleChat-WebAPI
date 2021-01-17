using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace SimpleChat.API.Config
{
    internal static class SwaggerConfig
    {
        internal static void Add(ref IServiceCollection services, IConfiguration Configuration)
        {
            var apiVersionDescriptionProvider = services.BuildServiceProvider().GetService<IApiVersionDescriptionProvider>();

            services.AddSwaggerGen(config =>
            {
                var title = "SimpleChat.API";
                var description = "This is a Web API for SimpleChat Application";
                var termsOfService = new Uri("https://github.com/SimpleChatApp/SimpleChat-WebAPI/blob/master/LICENSE");
                var license = new OpenApiLicense()
                {
                    Name = "MIT"
                };
                var contact = new OpenApiContact()
                {
                    Name = "Sevcan Alkan",
                    Email = "sevcanalkan@outlook.com.tr",
                    Url = new Uri("https://github.com/SevcanAlkan")
                };

                foreach (var versionDescription in apiVersionDescriptionProvider.ApiVersionDescriptions)
                {
                    config.SwaggerDoc(versionDescription.GroupName, new OpenApiInfo
                    {
                        Version = versionDescription.ApiVersion.ToString(),
                        Title = title + $" {versionDescription.ApiVersion.ToString()}",
                        Description = description,
                        TermsOfService = termsOfService,
                        License = license,
                        Contact = contact
                    });
                }

                // config.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                // config.CustomSchemaIds(x => x.FullName);

                // XML Documentation settings for the Swagger
                var xmlCommentsFile = $"{Assembly.GetAssembly(typeof(Program)).GetName().Name}.xml";
                var xmlCommentsFileFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);
                config.IncludeXmlComments(xmlCommentsFileFullPath);

                config.DocumentFilter<SwaggerDocsFilter>();
            });

            // services.Configure<ApiBehaviorOptions>(options =>
            // {
            //     options.InvalidModelStateResponseFactory = actionContext =>
            //     {
            //         var actionExecutingContext = actionContext as Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext;

            //         if (actionExecutingContext.ModelState.ErrorCount > 0
            //             && actionExecutingContext?.ActionArguments.Count == actionContext.ActionDescriptor.Parameters.Count)
            //         {
            //             return new UnprocessableEntityObjectResult(actionContext.ModelState);
            //         }

            //         return new BadRequestObjectResult(actionContext.ModelState);
            //     };
            // });
        }
    }
}
