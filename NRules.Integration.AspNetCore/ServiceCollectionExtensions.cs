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
    public static class RegistrationExtensions
    {

        public static IServiceCollection AddNRules(this IServiceCollection services, params Assembly[] assemblies)
        {
            var types = services.AddRules(assemblies);

            services.RegisterRepository(r => r.Load(x => x.From(types)))
                    .RegisterSessionFactory()
                    .RegisterSession();

            return services;
        }

        #region Rules

        private static Type[] AddRules(this IServiceCollection services, Assembly[] assemblies)
        {
            var scanner = new RuleTypeScanner();

            scanner.Assembly(assemblies);
            var ruleTypes = scanner.GetRuleTypes();

            services.AddManyScoped(ruleTypes);

            return ruleTypes;
        }

        #endregion

        #region Repository 
        private static IServiceCollection RegisterRepository(this IServiceCollection services, Action<RuleRepository> initAction)
        {
            services.AddScoped<IRuleActivator, AspNetCoreRuleActivator>();

            services.AddSingleton<IRuleRepository>(fac =>
            {
                var ruleRepo = new RuleRepository();
                initAction(ruleRepo);
                return ruleRepo;
            });

            return services;
        }
        #endregion

        #region SessionFactory
        private static IServiceCollection RegisterSessionFactory(this IServiceCollection services)
        {
            return services.RegisterSessionFactory(svc => svc.BuildServiceProvider().GetRequiredService<IRuleRepository>().Compile());

        }
        private static IServiceCollection RegisterSessionFactory(this IServiceCollection services, Func<IServiceCollection, ISessionFactory> compileFunc)
        {
            services.AddScoped<IDependencyResolver, AspNetCoreDependencyResolver>();

            return services.AddSingleton<ISessionFactory>(compileFunc(services));

        }
        #endregion

        #region Session
        private static IServiceCollection RegisterSession(this IServiceCollection services)
        {
            return services.RegisterSession(svc => svc.BuildServiceProvider().GetRequiredService<ISessionFactory>().CreateSession());
        }
        private static IServiceCollection RegisterSession(this IServiceCollection services, Func<IServiceCollection, ISession> compileFunc)
        {
            return services.AddScoped<ISession>(fac => { return compileFunc.Invoke(services); });

        }
        #endregion

        private static IServiceCollection AddManyScoped(this IServiceCollection services, Type[] types)
        {

            for (int i = 0; i < types.Length; i++)
            {
                services.AddScoped(types[i]);
            }
            return services;
        }

    }
}
