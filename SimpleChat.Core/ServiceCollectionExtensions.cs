using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleChat.Core
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterAllTypes<T>(this IServiceCollection services, Assembly[] assemblies,
            ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            var typesFromAssemblies = assemblies.SelectMany(a => a.DefinedTypes.Where(x => x.GetGenericTypeDefinition() == typeof(T)));
            foreach (var type in typesFromAssemblies)
                services.Add(new ServiceDescriptor(typeof(T), type, lifetime));
        }
    }
}