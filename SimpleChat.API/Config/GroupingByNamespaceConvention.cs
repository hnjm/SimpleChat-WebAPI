using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace SimpleChat.API.Config
{
    /// <summary>
    /// Fixes namespace of the controlles
    /// </summary>
    public class GroupingByNamespaceConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var controllerNamespace = controller.ControllerType.Namespace;
            var apiVersion = controllerNamespace.Split(".").Last().ToUpper();

            if (!apiVersion.StartsWith("V"))
                apiVersion = "V1.0";

            if (apiVersion.StartsWith("V"))
                apiVersion = apiVersion.Remove(0, 1);

            if (apiVersion.Length == 1)
                apiVersion += ".0";

            if (apiVersion.Contains('M'))
                apiVersion = apiVersion.Replace('M', '.');

            controller.ApiExplorer.GroupName = apiVersion;
            controller.ApiExplorer.IsVisible = true;
        }
    }
}
