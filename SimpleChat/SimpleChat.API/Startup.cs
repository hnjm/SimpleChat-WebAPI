using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SimpleChat.API.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SimpleChat.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers(options =>
            {
                options.Conventions.Add(new GroupingByNamespaceConvention());
            });

            #region API Versioning

            services.AddApiVersioning(o =>
            {
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.ApiVersionReader = new HeaderApiVersionReader("api-version");
                o.ReportApiVersions = true;
            });

            services.AddVersionedApiExplorer(options =>
            {
                    // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                    // note: the specified format code will format the version as "'v'major[.minor][-status]"
                    options.GroupNameFormat = "VV";

                    // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                    // can also be used to control the format of the API version in route templates
                    // options.SubstituteApiVersionInUrl = true;
            });

            #endregion

            #region Swagger

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

                config.SwaggerDoc("1.0", new OpenApiInfo
                {
                    Version = "1.0",
                    Title = title + " V1.0",
                    Description = description,
                    TermsOfService = termsOfService,
                    License = license,
                    Contact = contact
                });

                config.SwaggerDoc("1.1", new OpenApiInfo
                {
                    Version = "1.1",
                    Title = title + " V1.1",
                    Description = description,
                    TermsOfService = termsOfService,
                    License = license,
                    Contact = contact
                });

                // config.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                // config.CustomSchemaIds(x => x.FullName);

                // XML Documentation settings for the Swagger
                var xmlCommentsFile = $"{Assembly.GetAssembly(typeof(Program)).GetName().Name}.xml";
                var xmlCommentsFileFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);
                config.IncludeXmlComments(xmlCommentsFileFullPath);

                config.DocumentFilter<SwaggerDocsFilter>();
            });

            #endregion

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();
            app.UseDefaultFiles();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger(config =>
                {
                    config.RouteTemplate = "swagger/{documentName}/swagger.json";
                });
                app.UseSwaggerUI(config =>
                {
                    config.SwaggerEndpoint("/swagger/1.0/swagger.json", "1.0");
                    config.SwaggerEndpoint("/swagger/1.1/swagger.json", "1.1");
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
