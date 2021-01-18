using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimpleChat.API.Config;
using SimpleChat.API.SignalR;
using SimpleChat.ViewModel;
using System.Linq;
using System.Text;
using System.Text.Json;

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
            CorsConfig.Add(ref services, Configuration);
            APIControllersConfig.Add(ref services, Configuration);
            EntityFrameworkConfig.Add(ref services, Configuration);
            AuthenticationConfig.Add(ref services, Configuration);
            var mapper = AutoMapperConfig.Add(ref services, Configuration);
            APIVersioningConfig.Add(ref services, Configuration);
            SwaggerConfig.Add(ref services, Configuration);
            DependencyInjectionConfig.Add(ref services, Configuration, ref mapper);
            HealthChecksConfig.Add(ref services, Configuration);
            SignalRConfig.Add(ref services, Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider apiVersionDescriptionProvider)
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
                    foreach (var versionDescription in apiVersionDescriptionProvider.ApiVersionDescriptions)
                    {
                        config.SwaggerEndpoint($"/swagger/{versionDescription.GroupName}/swagger.json",
                            versionDescription.GroupName.ToUpperInvariant());
                    }

                    config.DefaultModelExpandDepth(2);
                    config.DisplayOperationId();
                    config.EnableDeepLinking();
                });
            }
            else
            {
            }

            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                // ResponseWriter = async (context, report) =>
                // {
                //     context.Response.ContentType = "application/json";

                //     var response = new HealthCheckResponseVM
                //     {
                //         Status = report.Status.ToString(),
                //         Checks = report.Entries.Select(x => new HealthCheckVM
                //         {
                //             Status = x.Value.Status.ToString(),
                //             Component = x.Key,
                //             Description = x.Value.Description
                //         }),
                //         Duration = report.TotalDuration
                //     };

                //     var bodyStr = JsonSerializer.Serialize(response);
                //     var bodyByteArray = Encoding.UTF8.GetBytes(bodyStr);
                //     await context.Response.Body.WriteAsync(bodyByteArray, 0, bodyByteArray.Length);
                // }

                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            // app.UseHealthChecksUI();

            //app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors(ConstantValues.DefaultCorsPolicy);

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                // endpoints.MapHealthChecks("/health");
                endpoints.MapHealthChecksUI(options =>
                {
                    options.UIPath = "/healthstatus";
                    options.ApiPath = "/health";
                });
                endpoints.MapHub<ChatHub>("/chathub");
            });
        }
    }
}
