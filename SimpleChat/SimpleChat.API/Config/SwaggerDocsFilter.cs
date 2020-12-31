using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SimpleChat.API.Config
{
    public class SwaggerDocsFilter : IDocumentFilter
    {
        // This can use to hide some controllers or actions
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            // foreach (var apiDescription in context.ApiDescriptions)
            // {
            //     // replace the data to your controller name
            //     if (apiDescription.ActionDescriptor.DisplayName.Contains("Data"))
            //     {
            //         var route = "/" + apiDescription.RelativePath.TrimEnd('/');
            //         swaggerDoc.Paths.Remove(route);
            //     }
            // }
        }
    }
}
