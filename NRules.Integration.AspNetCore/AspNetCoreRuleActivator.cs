using Microsoft.Extensions.DependencyInjection;
using NRules.Fluent;
using NRules.Fluent.Dsl;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRules.Integration.AspNetCore
{
    public class AspNetCoreRuleActivator : IRuleActivator
    {
        private readonly IServiceProvider _container;

        public AspNetCoreRuleActivator(IServiceProvider serviceProvider)
        {
            _container = serviceProvider;
        }
        public IEnumerable<Rule> Activate(Type type)
        {
            var requestedService = _container.GetService(type);

            if (requestedService != null)
            {
                var collectionType = typeof(IEnumerable<>).MakeGenericType(type);
                return (IEnumerable<Rule>)_container.GetRequiredService(collectionType);
            }

            return ActivateDefault(type);
        }

        private static IEnumerable<Rule> ActivateDefault(Type type)
        {
            yield return (Rule)Activator.CreateInstance(type);
        }
    }
}
