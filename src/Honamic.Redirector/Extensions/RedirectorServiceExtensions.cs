using System;
using System.Linq;
using Honamic.Redirector;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RedirectorServiceExtensions
    {
        public static IServiceCollection AddRedirector(this IServiceCollection services)
        {

            return services.AddRedirector(null);
        }

        public static IServiceCollection AddRedirector(this IServiceCollection services, Action<RedirectorOptions> configureOptions)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureOptions != null)
            {
                services.Configure(configureOptions);
            }

            if (!services.Any(c => c.ServiceType == typeof(IRedirectorStorage)))
            {
                services.TryAddScoped<IRedirectorStorage, OptionsRedirectorStorage>();
                services.TryAddSingleton<OptionsChangedHandler>();
            }

            services.TryAddSingleton<RedirectorManager>();

            return services;
        }
    }
}
