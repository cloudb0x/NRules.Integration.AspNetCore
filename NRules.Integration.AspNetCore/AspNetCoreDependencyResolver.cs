using Microsoft.Extensions.DependencyInjection;
using NRules.Extensibility;
using System;

namespace NRules.Integration.AspNetCore
{
    public class AspNetCoreDependencyResolver : IDependencyResolver
    {
        private readonly IServiceProvider _container;

        public AspNetCoreDependencyResolver(IServiceProvider serviceProvider)
        {
            _container = serviceProvider;
        }
        public object Resolve(IResolutionContext context, Type serviceType)
        {
            return _container.GetRequiredService(serviceType);
        }
    }
}
