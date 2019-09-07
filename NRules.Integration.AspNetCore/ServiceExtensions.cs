using Microsoft.Extensions.DependencyInjection;
using NRules.Extensibility;
using NRules.Fluent;
using NRules.RuleModel;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRules.Integration.AspNetCore
{
    public static class ServiceExtensions
    {

        public static IServiceCollection AddNRule(this IServiceCollection services, Assembly[] assemblies)
        {
            services.RegisterRepository(r =>
                      {
                          r.Load(x => x.From(services.AddRules(assemblies)));
                      })
                    .RegisterFactory()
                    .RegisterSession();

            return services;
        }

        #region Rules

        private static Type[] AddRules(this IServiceCollection services, Assembly[] assemblies)
        {
            var scanner = new RuleTypeScanner();

            scanner.Assembly(assemblies);
            var ruleTypes = scanner.GetRuleTypes();

            services.AddManySingletons(ruleTypes);

            return ruleTypes;
        }

        #endregion

        #region Repository 
        private static IServiceCollection RegisterRepository(this IServiceCollection services, Action<RuleRepository> initAction)
        {
            var ruleRepo = new RuleRepository();
            initAction(ruleRepo);
            services.AddSingleton(ruleRepo);

            return services;
        }
        #endregion

        #region Factory 
        private static IServiceCollection RegisterFactory(this IServiceCollection services)
        {

            return services.AddSingleton<ISessionFactory>(services.BuildServiceProvider().GetRequiredService<RuleRepository>().Compile());
        }

        #endregion

        #region Session
        private static IServiceCollection RegisterSession(this IServiceCollection services)
        {

            return services.AddScoped<ISession>(c => services.BuildServiceProvider().GetRequiredService<ISessionFactory>().CreateSession());
        }

        #endregion
        private static IServiceCollection AddManySingletons(this IServiceCollection services, Type[] types)
        {

            for (int i = 0; i < types.Length; i++)
            {
                services.AddSingleton(types[i]);
            }
            return services;
        }

    }
}
