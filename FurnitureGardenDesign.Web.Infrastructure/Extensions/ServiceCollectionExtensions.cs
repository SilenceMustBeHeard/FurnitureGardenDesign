using FurnitureGardenDesign.Services.Core;
using FurnitureGardenDesign.Services.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FurnitureGardenDesign.Web.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        private const string ServiceSuffix = "Service";

        public static IServiceCollection RegisterUserServices(
            this IServiceCollection services,
            Assembly serviceAssembly)
        {
            Type[] serviceClasses = serviceAssembly
                .GetTypes()
                .Where(t =>
                    t.IsClass &&
                    !t.IsAbstract &&
                    t.Name.EndsWith(ServiceSuffix))
                .ToArray();

            Type[] serviceInterfaces = serviceAssembly
                .GetTypes()
                .Where(t =>
                    t.IsInterface &&
                    t.Name.StartsWith("I") &&
                    t.Name.EndsWith(ServiceSuffix))
                .ToArray();

            foreach (Type implementation in serviceClasses)
            {
                Type? serviceInterface = serviceInterfaces
                    .FirstOrDefault(i =>
                        i.Name == $"I{implementation.Name}");

                if (serviceInterface == null)
                {
                    continue;
                }

                services.AddScoped(serviceInterface, implementation);
            }

            return services;
        }
    }

}

